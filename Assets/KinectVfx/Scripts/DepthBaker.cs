using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class DepthBaker : MonoBehaviour
{
    // CompteShader
    [SerializeField] ComputeShader _shader;
    [SerializeField] RenderTexture _colorMap = null;
    [SerializeField] RenderTexture _positionMap = null;
    [SerializeField] RenderTexture _depthMap = null;

    // kinect
    KinectSensor _sensor;
    MultiSourceManager _multiManager;

    // define
    int _colorWidth;
    int _colorHeight;
    int _depthWidth;
    int _depthHeight;
    int _count;

    // texutre
    RenderTexture _tempColorMap;
    RenderTexture _tempPositionMap;
    RenderTexture _tempDepthMap;

    ComputeBuffer _depthBuffer;
    ComputeBuffer _colorSpaceBuffer;

    // parameter
    [HideInInspector] public float threshold = 0.0f;

    void Awake()
    {
        threshold = PlayerPrefs.GetFloat("THRESHOLD");
    }

    // Start is called before the first frame update
    void Start()
    {
        _sensor = KinectSensor.GetDefault();
        if (_sensor != null)
        {
            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
        }

        _multiManager = GetComponent<MultiSourceManager>();

        _colorWidth = _sensor.ColorFrameSource.FrameDescription.Width;
        _colorHeight = _sensor.ColorFrameSource.FrameDescription.Height;
        _depthWidth = _sensor.DepthFrameSource.FrameDescription.Width;
        _depthHeight = _sensor.DepthFrameSource.FrameDescription.Height;
        _count = _depthWidth * _depthHeight;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDepthBuffer();
        UpdateColorSpace();
        UpdateComputeShader();

        Graphics.CopyTexture(_tempColorMap, _colorMap);
        Graphics.CopyTexture(_tempPositionMap, _positionMap);
        Graphics.CopyTexture(_tempDepthMap, _depthMap);
    }

    void OnApplicationQuit()
    {
        SavePlayerPrefs();
    }

    void UpdateDepthBuffer()
    {
        var depthData = _multiManager.GetDepthData();
        uint[] rawData = new uint[_count / 2];
        Buffer.BlockCopy(depthData, 0, rawData, 0, _count * 2);
        _depthBuffer = new ComputeBuffer(_count / 2, sizeof(uint));
        _depthBuffer.SetData(rawData);
    }

    void UpdateColorSpace()
    {
        var depthData = _multiManager.GetDepthData();
        ColorSpacePoint[] colorSpace = new ColorSpacePoint[depthData.Length];
        _sensor.CoordinateMapper.MapDepthFrameToColorSpace(depthData, colorSpace);

        int[] colorPos = new int[_count * 2];
        for (int i = 0; i < colorSpace.Length; i++)
        {
            colorPos[i] = (int)(colorSpace[i].X + 0.5f);
            colorPos[i + _count] = (int)(colorSpace[i].Y + 0.5f);
        }
        _colorSpaceBuffer = new ComputeBuffer(_count * 2, sizeof(uint));
        _colorSpaceBuffer.SetData(colorPos);
    }

    void UpdateComputeShader()
    {
        if (_tempColorMap == null)
        {
            _tempColorMap = new RenderTexture(_depthWidth, _depthHeight, 0, RenderTextureFormat.ARGB32);
            _tempColorMap.enableRandomWrite = true;
            _tempColorMap.Create();
        }
        if (_tempPositionMap == null)
        {
            _tempPositionMap = new RenderTexture(_depthWidth, _depthHeight, 0, RenderTextureFormat.ARGB32);
            _tempPositionMap.enableRandomWrite = true;
            _tempPositionMap.Create();
        }
        if (_tempDepthMap == null)
        {
            _tempDepthMap = new RenderTexture(_depthWidth, _depthHeight, 0, RenderTextureFormat.ARGB32);
            _tempDepthMap.enableRandomWrite = true;
            _tempDepthMap.Create();
        }

        int kernelID = _shader.FindKernel("CSMain");

        _shader.SetFloat("Threshold", threshold);
        _shader.SetBuffer(kernelID, "DepthBuffer", _depthBuffer);
        _shader.SetBuffer(kernelID, "ColorSpaceBuffer", _colorSpaceBuffer);
        _shader.SetTexture(kernelID, "ColorMap", _tempColorMap);
        _shader.SetTexture(kernelID, "PositionMap", _tempPositionMap);
        _shader.SetTexture(kernelID, "ColorTexture", _multiManager.GetColorTexture());
        _shader.SetTexture(kernelID, "DepthMap", _tempDepthMap);
        _shader.Dispatch(kernelID, _depthWidth / 8, _depthHeight / 8, 1);

        _depthBuffer.Release();
        _colorSpaceBuffer.Release();
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetFloat("THRESHOLD", threshold);
        PlayerPrefs.Save();
    }
}
