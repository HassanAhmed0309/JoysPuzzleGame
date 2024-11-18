using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region PlayerInputActions Object
    PlayerInputActions PlayerAction;
    #endregion

    #region Player Action Map
    public InputActionMap Player;
    public InputAction PlayerMoveGhost;
    public InputAction PlayerFloatup;
    public InputAction PlayerFloatDown;
    public InputAction PlayerLook;
    public InputAction PlayerPossess;
    public InputAction PlayerShooting;
    public InputAction PlayerAiming;
    public InputAction PlayerJumping;
    #endregion

    #region Vehicle Action Map 
    public InputAction VehicleMovement;
    public InputAction VehicleRaiseLower;
    public InputAction VehiclePickDrop;
    public InputAction VehicleRotateClockwise;
    public InputAction VehicleRotateAntiClockwise;
    #endregion

    #region Airborne Action Map
    public InputAction AirborneMovement;
    public InputAction AirborneUp;
    public InputAction AirborneDown;
    public InputAction AirborneRotateClockwise;
    public InputAction AirborneAntiClockwise;
    #endregion

    #region Ghost FLoat Bools
    public bool IsFloatingUp = false;
    public bool IsFloatingDown = false;
    #endregion

    #region Possession Script Reference
    //Reference to possession feature for subscribing the "Possess" function
    public Possession possession;
    #endregion

    #region Gun Script Reference
    //Reference to gun feature for subscribing the "Shoot" function
    public Gun gunScript;
    #endregion

    #region Pickup Script Reference
    //Reference to gun feature for subscribing the "Pickup" function
    public PickUp pickUp;
    #endregion

    #region Airborne Controller Reference
    //Reference to airborne controller for changing the bools of float up and down
    public AirborneController airborneController;
    #endregion

    #region SwitchVcam reference
    public SwitchVcam vcam;
    #endregion

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        PlayerAction = new PlayerInputActions();

        Player = PlayerAction.Player;
        PlayerMoveGhost = PlayerAction.Player.MoveGhost;
        PlayerFloatup = PlayerAction.Player.FloatUp;
        PlayerFloatDown = PlayerAction.Player.FloatDown;
        PlayerLook = PlayerAction.Player.Look;
        PlayerPossess = PlayerAction.Player.Posses;
        PlayerShooting = PlayerAction.Player.Shooting;
        PlayerAiming = PlayerAction.Player.Aiming;
        PlayerJumping = PlayerAction.Player.Jump;

        VehicleMovement = PlayerAction.Vehicle.Movement;
        VehicleRaiseLower = PlayerAction.Vehicle.RaiseLower;
        VehiclePickDrop = PlayerAction.Vehicle.PickDrop;
        VehicleRotateClockwise = PlayerAction.Vehicle.RotateClockwise;
        VehicleRotateAntiClockwise = PlayerAction.Vehicle.RotateAntiClockwise;

        AirborneMovement = PlayerAction.AirborneVehicles.Movement;
        AirborneUp = PlayerAction.AirborneVehicles.Up;
        AirborneDown = PlayerAction.AirborneVehicles.Down;
        AirborneRotateClockwise = PlayerAction.AirborneVehicles.RotateClockwise;
        AirborneAntiClockwise = PlayerAction.AirborneVehicles.RotateAntiClockwise;
    }
    #endregion

    #region GhostMovement Handling
    [Client]
    public void EnableGhostMovement()
    {
        PlayerFloatup.performed += context =>
        {
            IsFloatingUp = true;
        };
        PlayerFloatup.canceled += context =>
        {
            IsFloatingUp = false;
        };
        
        PlayerFloatDown.performed += context =>
        {
            IsFloatingDown = true;
        };
        PlayerFloatDown.canceled += context =>
        {
            IsFloatingDown = false;
        };

        PlayerMoveGhost.Enable();
        PlayerFloatup.Enable();
        PlayerFloatDown.Enable();
    }
    [Client]
    public void DisableGhostMovement()
    {
        PlayerMoveGhost.Disable();
        PlayerFloatup.Disable();
        PlayerFloatDown.Disable();
    }
    #endregion 

    #region Possession Feature Handling
    [Client]
    public void EnablePossessionFeature()
    {
        PlayerPossess.performed += possession.Posses;
        PlayerPossess.Enable();
    }
    [Client]
    public void DisablePossessionFeature()
    {
        PlayerPossess.performed -= possession.Posses;
        PlayerPossess.Disable();
    }
    #endregion
        
    #region Shooting Feature Handling
    public void EnableShootingFeature()
    {
        PlayerShooting.performed += gunScript.Shoot;
        PlayerShooting.Enable();
    }
    public void DisableShootingFeature()
    {
        PlayerShooting.performed -= gunScript.Shoot;
        PlayerShooting.Disable();
    }
    #endregion

    #region PickUp Feature Handling
    public void EnablePickupFeature()
    {
        VehiclePickDrop.performed += pickUp.Pickup;
    }
    public void DisablePickupFeature()
    {
        VehiclePickDrop.performed -= pickUp.Pickup;
    }
    #endregion

    #region Helicopter FlyUp and Down and Rotation functions
    public void FlyUp()
    {
        AirborneUp.performed += context =>
        {
            airborneController.IsFloatingUp = true;
        };
        AirborneUp.canceled += context =>
        {
            airborneController.IsFloatingUp = false;
        };
        AirborneUp.Enable();
    }

    public void FlyDown()
    {
        AirborneDown.performed += context =>
        {
            airborneController.IsFloatingDown = true;
        };
        AirborneDown.canceled += context =>
        {
            airborneController.IsFloatingDown = false;
        };
        AirborneDown.Enable();
    }

    public bool PressedRotateClockwise()
    {
        AirborneRotateClockwise.Enable();
        return AirborneRotateClockwise.IsPressed();
    }

    public bool PressedRotateAntiClockwise()
    {
        AirborneAntiClockwise.Enable();
        return AirborneAntiClockwise.IsPressed();
    }
    
    public void EnableAirborneVehicleMovement()
    {
        AirborneUp.Enable();
        AirborneMovement.Enable();
        AirborneDown.Enable();
    }
    
    public void DisableAirborneVehicleMovement()
    {
        AirborneUp.Disable();
        AirborneMovement.Disable();
        AirborneDown.Disable();
    }
    #endregion

    #region Vehicle Movement and Rotation
    public void EnableVehicleMovement()
    {
        VehicleMovement.Enable();
    }
    public void DisableVehicleMovement()
    {
        VehicleMovement.Disable();
    }
    public bool VehicleRotateClockwiseMovement()
    {
        VehicleRotateClockwise.Enable();
        return VehicleRotateClockwise.IsPressed();
    }
    public bool VehicleRotateAntiClockwiseMovement()
    {
        VehicleRotateAntiClockwise.Enable();
        return VehicleRotateAntiClockwise.IsPressed();
    }
    #endregion

    #region SwitchVcam Feature
    public void EnableAim()
    {
        PlayerAiming.performed += vcam.StartAim;
        PlayerAiming.canceled += vcam.CancelAim;
        PlayerAiming.Enable();
    }

    public void DisableAim()
    {
        PlayerAiming.performed -= vcam.StartAim;
        PlayerAiming.canceled -= vcam.CancelAim;
        PlayerAiming.Disable();
    }
    #endregion
}
