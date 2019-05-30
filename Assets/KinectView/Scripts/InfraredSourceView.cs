﻿using UnityEngine;
using System.Collections;

public class InfraredSourceView : MonoBehaviour
{
    public GameObject InfraredSourceManager;
    private InfraredSourceManager _InfraredManager;

    void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_UnlitColorMap", new Vector2(-1, 1));
    }

    void Update()
    {
        if (InfraredSourceManager == null)
        {
            return;
        }

        _InfraredManager = InfraredSourceManager.GetComponent<InfraredSourceManager>();
        if (_InfraredManager == null)
        {
            return;
        }
        
        gameObject.GetComponent<Renderer>().material.SetTexture("_UnlitColorMap", _InfraredManager.GetInfraredTexture());
    }
}
