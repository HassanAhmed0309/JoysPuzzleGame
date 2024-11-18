using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarSnapping : MonoBehaviour
{
    public bool CanBeSnapped = false;
    public bool isSnapped = false;

    [SerializeField]
    Possession CarPossession;

    [SerializeField]
    GameObject CarPrefab;

    [SerializeField]
    Rigidbody SoliderRb;
    [SerializeField]
    GameObject SoliderCollider;
    [SerializeField]
    Gun gun;
    [SerializeField]
    private CinemachineVirtualCamera VCam;

    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator TurnFalseBool()
    {
        yield return new WaitForSeconds(2);
        CanBeSnapped = false;
    }

    private void Update()
    {
        if(!isSnapped && (Mathf.Abs(transform.localRotation.x) >= 1 || Mathf.Abs(transform.localRotation. z) >= 1))
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.y, 0f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(TurnFalseBool());

        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.name == "ToyCar")
        {
            if (collision.gameObject.GetComponent<EnumsID>() != null && collision.gameObject.GetComponent<EnumsID>().Enum == Toys.ToyCar && CanBeSnapped)
            {
                Debug.Log("Touched " + collision.gameObject.name);
                transform.SetParent(collision.transform);
                
                isSnapped = true;

                transform.SetLocalPositionAndRotation(new Vector3(0f, 1.6f, 0f), Quaternion.Euler(Vector3.zero));
                SoliderCollider.SetActive(false);
                Destroy(SoliderRb);

                CarPrefab.GetComponent<Rigidbody>().centerOfMass = new Vector3(1f, 1f, 1f);
            }
        } 
    }
}
