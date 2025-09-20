using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CaptureCameraView : MonoBehaviour
{
    [SerializeField] private RenderTexture outputTexture;
    private void Start()
    {
        Camera.main.targetTexture = outputTexture;
    }
}
