using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRef : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.localCamera = this.GetComponent<CinemachineFreeLook>();
    }
}
