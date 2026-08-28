using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasAspectRatio : MonoBehaviour
{
    [SerializeField] CanvasScaler scaler;


    private void Update() {
        float aspectRatio = (float)Screen.width / (float)Screen.height;

        Debug.Log($"{aspectRatio} -> {Screen.width}x{Screen.height}");
        scaler.matchWidthOrHeight = aspectRatio == 1? 0.5f : aspectRatio < 0.6f ? 0 : 1;
    }
}
