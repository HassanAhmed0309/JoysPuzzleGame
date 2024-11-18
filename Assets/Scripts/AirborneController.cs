using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneController : NetworkBehaviour
{
    #region Variables
    #region InputManager reference
    private InputManager inputManager;
    #endregion

    #region Player Rigidbody
    private Rigidbody Rb;
    #endregion

    #region Float controlling variables
    [SyncVar] public bool IsFloatingUp = false;
    [SyncVar] public bool IsFloatingDown = false;
    [SerializeField]
    private float JumpForce = 5f;
    #endregion

    #region Movement and Jump related variables
    [SerializeField]
    private float MovementForce = 1f;
    [SerializeField]
    private float MaxSpeed = 5f;
    
    private Vector3 ForceDirection = Vector3.zero;

    [SyncVar]
    public float HelicopterRotationSpeed = .5f;
    [SyncVar]
    public float initialForce = 10f;
    public float forceIncreaseRate = 2f;
    [SyncVar]
    float val = 0f;
    #endregion

    #region This GameObject
    public AirborneController ThisScript;
    public GameObject Rotor;
    public GameObject RotorBack;
    [SyncVar] public float RotorSpeed = 1f;
    #endregion

    #region Player Camera
    public Camera PlayerCamera;
    //public Cinemachine.CinemachineFreeLook CinemachineCamera;
    //CameraManager cameraManager;
    #endregion

    #region Possession
    [SerializeField]
    private Possession HelicopterPossession;
    #endregion

    #region Authority
    public Authority Authority;
    #endregion

    #region Rigidbody
    public Rigidbody currentRB;
    #endregion

    #region Crane Handling
    [SerializeField]
    CraneHandle craneHandle;
    [SerializeField]
    PickUp PickUp;
    #endregion

    #endregion

    #region Unity Functions
    private void OnEnable()
    {
        if (GetComponent<EnumsID>().Enum == Toys.Helicopter && !isOwned)
        {
            var clientID = HelicopterPossession.Ghost.GetComponent<NetworkIdentity>();
            var HelicopterID = netIdentity;
            if (!isOwned && clientID.isLocalPlayer)
            {
                Authority auth = HelicopterPossession.Ghost.GetComponent<Authority>();
                auth.CmdAssignAuthority(clientID, HelicopterID);
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

        val = transform.position.y;
        
        Rb = GetComponent<Rigidbody>();
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        inputManager.airborneController = this;
        inputManager.FlyUp();
        
        if (transform.position.y > val + 1f)
        {
            inputManager.FlyDown();
        }

        inputManager.EnableAirborneVehicleMovement();

        if (isOwned) { Rb.isKinematic = false; }
    }
    //[ClientCallback]
    //private void OnEnable()
    //{
    //    //currentRB.centerOfMass = Vector3.zero;
    //    //currentRB.inertiaTensor = Vector3.one;


    //    //cameraManager = new CameraManager();
    //    //cameraManager.ChangeCamera(CinemachineCamera, this.gameObject);
    //}

    private void FixedUpdate()
    {
        if(isOwned)
        {
            Movement();
        }
    }

    private void OnDisable()
    {
        if(inputManager != null)
            inputManager.DisableAirborneVehicleMovement();
        //CinemachineCamera.m_Lens.FieldOfView = 40;
        craneHandle.enabled = false; 
        PickUp.enabled = false;
    }
    #endregion
    
    #region Movement main functions
    private void Movement()
    {
        if (transform.position.y > val + 1f)
        {
            inputManager.FlyDown();
        }

        Rb.useGravity = false;
        
        //Helicopter Floating Up
        if (IsFloatingUp)
        {
            ForceDirection += Vector3.up * JumpForce;
            Rb.AddForce(ForceDirection);
        }

        //Helicopter Floating Down
        if (IsFloatingDown)
        {
            ForceDirection += Vector3.down * JumpForce;
            Rb.AddForce(ForceDirection);
        }

        if (transform.position.y > val + 1f)
        {
            craneHandle.enabled = true;
            PickUp.enabled = true;
            inputManager.AirborneMovement.Enable();
            ForceDirection += inputManager.AirborneMovement.ReadValue<Vector2>().y * transform.forward * MovementForce;
            ForceDirection += inputManager.AirborneMovement.ReadValue<Vector2>().x * transform.right * MovementForce;
            Rb.AddForce(ForceDirection);
            ForceDirection = Vector3.zero;

            //Limits the force on the object.
            Vector3 horizontalVelocity = Rb.velocity;
            horizontalVelocity.y = 0;
            if (horizontalVelocity.sqrMagnitude > MaxSpeed * MaxSpeed)
            {
                Rb.velocity = horizontalVelocity.normalized * MaxSpeed + transform.up * Rb.velocity.y;
            }

            if (inputManager.PressedRotateAntiClockwise())
            {
                transform.Rotate(-1 * transform.up * HelicopterRotationSpeed * Time.deltaTime);
            }
            else if (inputManager.PressedRotateClockwise())
            {
                transform.Rotate(transform.up * HelicopterRotationSpeed * Time.deltaTime);
            }
            if (!(transform.rotation.eulerAngles.z <= 1f && transform.rotation.eulerAngles.z >= 0f) ||
                !(transform.rotation.eulerAngles.x <= 1f && transform.rotation.eulerAngles.x >= 0f))
            {
                transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f)));
            }
        }
    }
    #endregion
}
