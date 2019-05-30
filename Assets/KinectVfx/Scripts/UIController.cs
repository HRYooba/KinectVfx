using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    CanvasGroup _canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = _canvas.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            _canvasGroup.alpha = _canvasGroup.alpha == 0.0f ? _canvasGroup.alpha = 1.0f : _canvasGroup.alpha = 0.0f;
        }
    }
}
