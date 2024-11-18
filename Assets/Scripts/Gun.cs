using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class Gun : NetworkBehaviour
{
    InputManager inputManager;

    [SerializeField]
    private GameObject SoliderGameObject;
    [SerializeField]
    private Possession soliderPossession;

    public Transform bulletSpawnPoint;
    public Transform bulletSpawnPoint1;

    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    public float HitAndMissDistance = 25f;

    GameObject bullt;

    public Gun thisGunScript;

    public Camera MainCamera;

    public GameObject crosshair;

    public Animator animator;

    Vector3 targeted;
    private void OnEnable()
    {
        if (SoliderGameObject != null && SoliderGameObject.GetComponent<EnumsID>().Enum == Toys.Solider)
        {
            var clientID = soliderPossession.Ghost.GetComponent<NetworkIdentity>();
            var SoliderID = netIdentity;
            if (!isOwned && clientID.isLocalPlayer)
            {
                Authority auth = soliderPossession.Ghost.GetComponent<Authority>();
                auth.CmdAssignAuthority(clientID, SoliderID);
            }
        }
    }

    public override void OnStartAuthority()
    {
        Debug.Log("Gave Authority " + this.netId + " " + this.name);

        Debug.Log("Name:" + this.name +
            "\nIs Client:" + isClient +
            "\nIs Server:" + isServer +
            "\nIs Owned:" + isOwned +
            "\nIs Local Player:" + isLocalPlayer);

        if (SoliderGameObject != null)
        {
            MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
            inputManager.gunScript = this;
            inputManager.EnableShootingFeature();
            crosshair.SetActive(true);
        }
    }

    private void Update()
    {
        if(isOwned)
        {
            SoldierMovement();
        }
    }

    [Client]
    private void SoldierMovement()
    {
        if (SoliderGameObject != null && !SoliderGameObject.transform.GetComponent<CarSnapping>().isSnapped)
        {
            SoliderGameObject.transform.SetLocalPositionAndRotation(SoliderGameObject.transform.localPosition, MainCamera.transform.localRotation);
        }
    }

    [Client]
    public void Shoot(InputAction.CallbackContext callback)
    {
        animator.SetBool("Shoot", true);
        CmdBulletCreation();
        animator.SetBool("Shoot", false);
    }

    [Command(requiresAuthority = false)]
    void CmdBulletCreation()
    {
        GameObject reference = Instantiate(bulletPrefab,bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        reference.name = bulletPrefab.name;
        if (SoliderGameObject.name == "Solider")
        {
            targeted = transform.forward;
        }
        else if (SoliderGameObject.name == "AI Bot")
        {
            targeted = bulletSpawnPoint.transform.forward;
        }
        Rigidbody rb = reference.GetComponent<Rigidbody>();
        rb.AddForce(targeted * bulletSpeed * bulletSpeed * Time.deltaTime, ForceMode.VelocityChange);
        NetworkServer.Spawn(reference);
    }

    public void ShootBasedOnObject(GameObject anyToy)
    {
        RaycastHit hit;
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        var bullet1 = Instantiate(bulletPrefab, bulletSpawnPoint1.position, bulletSpawnPoint1.rotation);

        Bullet bullt = bullet.GetComponent<Bullet>();
        Bullet bullt1 = bullet1.GetComponent<Bullet>();
        if (Physics.Raycast(anyToy.transform.position, anyToy.transform.forward, out hit, Mathf.Infinity))
        {
            bullt.target = bulletSpawnPoint.transform.forward;
            bullt.Name = "AI Bot";
            bullt1.target = bulletSpawnPoint1.transform.forward;
            bullt1.Name = "AI Bot";
        }
        else
        {
            bullt.target = bulletSpawnPoint.transform.forward;
            bullt.Name = "AI Bot";

            bullt1.target = bulletSpawnPoint1.transform.forward;
            bullt1.Name = "AI Bot";

        }
    }

    private void OnDisable()
    {
        if(SoliderGameObject != null && crosshair != null)
        {
            inputManager.DisableShootingFeature();
            crosshair.SetActive(false);
        }
    }
}
