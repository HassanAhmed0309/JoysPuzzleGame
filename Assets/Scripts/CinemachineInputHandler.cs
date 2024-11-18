using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineInputHandler : MonoBehaviour
{
    //private Input inputSystem;
    //private Vector2 LookCamera; // your LookDelta
    //public float deadZoneX = 0.2f;

    //private void Awake()
    //{
    //    inputSystem = new Input();

    //    inputSystem.PlayerGameplay.CameraControl.performed += ctx => LookCamera = ctx.ReadValue<Vector2>().normalized;
    //    inputSystem.PlayerGameplay.CameraControl.performed += ctx => GetInput();
    //}

    //private void GetInput()
    //{
    //    CinemachineCore.GetInputAxis = GetAxisCustom;
    //}

    //public float GetAxisCustom(string axisName)
    //{
    //    // LookCamera.Normalize();

    //    if (axisName == "CamControlX")
    //    {
    //        if (LookCamera.x > deadZoneX || LookCamera.x < -deadZoneX) // To stabilise Cam and prevent it from rotating when LookCamera.x value is between deadZoneX and - deadZoneX
    //        {
    //            return LookCamera.x;
    //        }
    //    }

    //    else if (axisName == "CamControlY")
    //    {
    //        return LookCamera.y;
    //    }

    //    return 0;
    //}

    //private void OnEnable()
    //{
    //    inputSystem.PlayerGameplay.Enable();
    //}

    //private void OnDisable()
    //{
    //    inputSystem.PlayerGameplay.Disable();
    //}
}
