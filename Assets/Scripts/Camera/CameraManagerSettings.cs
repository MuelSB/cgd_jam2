using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CameraManagerSettings
{
    public Vector3 CameraEulerRotation = new Vector3(54.0f, 0.0f, 0.0f);
    public float cameraLerpSpeed = 100.0f;
    public float cameraYPosition = 40.0f;
    public float cameraZOffset = -20.0f;
}
