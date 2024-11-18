using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : NetworkBehaviour
{
    #region Possession
    public Possession currentpossesion;
    #endregion

    #region Authority
    public Authority Authority;
    #endregion

    #region Player Rigidbody
    private Rigidbody Rb;
    #endregion

    #region Input Manager Reference
    InputManager inputManager;
    #endregion

    #region Movement and Jump related variables
    [SyncVar]
    [SerializeField]
    private float MovementForce = 1f;
    [SyncVar]
    [SerializeField]
    private float JumpForce = 5f;
    [SyncVar]
    [SerializeField]
    private float MaxSpeed = 5f;
    private Vector3 ForceDirection = Vector3.zero;
    #endregion

    #region Player Camera
    Camera PlayerCamera;
    #endregion

    #region Animation Variables
    public Animator Animator;
    #endregion

    #region Unity Functions
    private void OnEnable()
    {
        if (GetComponent<EnumsID>().Enum == Toys.Bear && !this.isOwned)
        {
            var clientID = currentpossesion.Ghost.GetComponent<NetworkIdentity>();
            var BearID = netIdentity;
            if (!isOwned && clientID.isLocalPlayer)
            {
                Authority auth = currentpossesion.Ghost.GetComponent<Authority>();
                auth.CmdAssignAuthority(clientID, BearID);
            }
        }
        else
        {
            Debug.Log("On Enable");
            inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
            if (inputManager != null)
            {
                inputManager.EnableGhostMovement();
            }
            else
                Debug.Log("1. Input Manager was found null");
        }
    }

    private void OnDisable()
    {
        if(inputManager != null)
            inputManager.DisableGhostMovement();
    }

    private void FixedUpdate()
    {
        if(isOwned)
        {
            Movement();
        }
    }
    #endregion

    #region Multiplayer Function
    public override void OnStartAuthority()
    {
        Debug.Log("Gave Authority " + this.netId + " " + this.name);

        Debug.Log("Name:" + this.name +
            "\nIs Client:" + isClient +
            "\nIs Server:" + isServer + 
            "\nIs Owned:" + isOwned +
            "\nIs Local Player:" + isLocalPlayer);

        Rb = GetComponent<Rigidbody>();

        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        if (inputManager != null)
        {
            inputManager.EnableGhostMovement();
        }
        else
            Debug.Log("2. Input Manager was found null");

        PlayerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (GetComponent<EnumsID>().Enum == Toys.Bear)
        {
            Rb.centerOfMass = Vector3.zero;
        }
        
        if (!enabled)
            enabled = true;
        else
        {
            if (isOwned) { Rb.isKinematic = false; }
        }
        //Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion

    #region Movement main functions
    private void Movement()
    {
        Toys CacheEnum = GetComponent<EnumsID>().Enum;
        
        if (CacheEnum == Toys.Ghost)
        {
            inputManager.PlayerMoveGhost.Enable();
            //Debug.Log("Movement Vector: " + inputManager.PlayerMoveGhost.ReadValue<Vector2>());
            if (inputManager.PlayerMoveGhost.ReadValue<Vector2>().y == 0 
                && inputManager.PlayerMoveGhost.ReadValue<Vector2>().x == 0)
            {
                Animator.SetBool("IsWalking", false);
            }
            else
            {
                //Debug.Log("Movement: " + Move.ReadValue<Vector2>().x + " , " + Move.ReadValue<Vector2>().y);
                ForceDirection += inputManager.PlayerMoveGhost.ReadValue<Vector2>().y * GetCameraForward(PlayerCamera) * MovementForce;
                ForceDirection += inputManager.PlayerMoveGhost.ReadValue<Vector2>().x * GetCameraRight(PlayerCamera) * MovementForce;
                //Debug.Log("Camera: " + GetCameraForward(PlayerCamera) + " , " + GetCameraRight(PlayerCamera));
                Animator.SetBool("IsWalking", true);
            }

            //Ghost Floating Up
            if (inputManager.IsFloatingUp)
            {
                Animator.SetBool("IsWalking", false);
                ForceDirection += transform.up * JumpForce;
                Rb.AddForce(ForceDirection, ForceMode.Impulse);
            }

            //Ghost Floating Down
            if (inputManager.IsFloatingDown)
            {
                Animator.SetBool("IsWalking", false);
                ForceDirection += Vector3.down * JumpForce;
                Rb.AddForce(ForceDirection, ForceMode.Impulse);
            }
        }
        
        else if (CacheEnum == Toys.Bear)
        {
            if (inputManager.PlayerMoveGhost.ReadValue<Vector2>().y != 0 || inputManager.PlayerMoveGhost.ReadValue<Vector2>().x != 0)
            {
                Animator.SetBool("isWalking", true);
                ForceDirection += inputManager.PlayerMoveGhost.ReadValue<Vector2>().y * GetCameraForward(PlayerCamera) * MovementForce;
                ForceDirection += inputManager.PlayerMoveGhost.ReadValue<Vector2>().x * GetCameraRight(PlayerCamera) * MovementForce;
            }
            else
            {
                Animator.SetBool("isWalking", false);
                Rb.velocity = new Vector3(0f, 0f, 0f);
            }
        }
        //Debug.Log("Force Direction Vector: " + ForceDirection);
        Rb.AddForce(ForceDirection, ForceMode.Impulse);
        ForceDirection = Vector3.zero;
        //Limits the force on the object.
        Vector3 horizontalVelocity = Rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > MaxSpeed * MaxSpeed)
        {
            Rb.velocity = horizontalVelocity.normalized * MaxSpeed + transform.up * Rb.velocity.y;
        }
        //Works on character rotation as per the movement of the character
        LookAt();
    }
    
    private void LookAt()
    {
        //Debug.Log("Look at me");
        Vector3 direction = Rb.velocity;
        direction.y = 0f;

        if (inputManager.PlayerMoveGhost.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            Rb.rotation = Quaternion.LookRotation(direction, gameObject.transform.up);
        }
        else
        {
            Rb.angularVelocity = Vector3.zero;
        }
    }
    
    private Vector3 GetCameraForward(Camera PlayerCamera)
    {
        Vector3 forward = PlayerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera PlayerCamera)
    {
        Vector3 right = PlayerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }
    #endregion
}
