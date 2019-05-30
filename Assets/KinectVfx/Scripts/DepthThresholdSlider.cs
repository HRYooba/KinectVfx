using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthThresholdSlider : MonoBehaviour
{
    [SerializeField] DepthBaker _depthBaker;
    Slider _slider;

    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.value = _depthBaker.threshold;
    }

    // Update is called once per frame
    void Update()
    {
        _depthBaker.threshold = _slider.value;
    }
}
