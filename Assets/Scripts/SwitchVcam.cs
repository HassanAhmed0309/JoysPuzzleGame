using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using Mirror;

public class SwitchVcam : MonoBehaviour
{
    InputManager inputManager;

    [SerializeField]
    private int PriorityBoostAmount = 10;

    [SerializeField]
    private CinemachineVirtualCamera VCam;

    [SerializeField]
    private GameObject ThirdPersonCamera;
    [SerializeField]
    private GameObject AimCamera;
    [SerializeField]
    private Canvas ThirdPersonCanvas;
    [SerializeField]
    private Canvas AimCanvas;

    [SerializeField]
    GameObject Solider;

    // Start is called before the first frame update
    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        inputManager.vcam = this;
    }

    private void OnEnable()
    {
        ThirdPersonCamera.SetActive(true);
        AimCamera.SetActive(true);

        inputManager.EnableAim();
    }

    private void OnDisable()
    {
        inputManager.DisableAim();
    }

    public void CancelAim(InputAction.CallbackContext context)
    {
        VCam.Priority -= PriorityBoostAmount;
        AimCanvas.gameObject.SetActive(false);
        ThirdPersonCanvas.gameObject.SetActive(true);
    }

    public void StartAim(InputAction.CallbackContext context)
    {
        VCam.Priority += PriorityBoostAmount;
        AimCanvas.gameObject.SetActive(true);
        ThirdPersonCanvas.gameObject.SetActive(false);
    }
}
