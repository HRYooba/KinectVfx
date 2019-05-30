using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorSourceView : MonoBehaviour
{
    public GameObject ColorSourceManager;
    private ColorSourceManager _ColorManager;

    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_UnlitColorMap", new Vector2(-1, 1));
    }

    void Update()
    {
        if (ColorSourceManager == null)
        {
            return;
        }

        _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
        if (_ColorManager == null)
        {
            return;
        }

        gameObject.GetComponent<Renderer>().material.SetTexture("_UnlitColorMap", _ColorManager.GetColorTexture());
    }
}
