using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private Cinemachine.CinemachineFreeLook freeLook;
    public override void OnStartAuthority()
    {
        freeLook.gameObject.SetActive(true);
        enabled = true;
    }
}
