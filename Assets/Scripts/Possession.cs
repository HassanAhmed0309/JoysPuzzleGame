using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using Mirror;

public class Possession : NetworkBehaviour
{
    #region Input Manager variables
    private InputManager inputManager;
    #endregion

    #region UI Variables
    [Header("UI Variables")]
    GameObject UIHandler;
    [SerializeField]
    private GameObject OverallUI;
    public GameObject Message;
    #endregion

    #region Ghost Variables
    [Header("Ghost Variables")]
    [SyncVar]
    public GameObject Ghost;
    public ThirdPersonController ghostController;
    #endregion

    #region ThirdPerson Camera Variables
    [Header("Third Person Camera")]
    [SyncVar]
    public GameObject OverAllCamera;
    [SerializeField]
    CameraManager cameraManager;
    #endregion

    #region Slider Controls variables
    [Header("Slider Controls")]
    [SerializeField]
    private Slider MainSlider;
    public float startingValue;
    public float endValue;
    public float minValue;
    #endregion

    #region Possession Boolean Variables
    [Header("Possession Boolean Variables")]    
    [SyncVar]
    public bool IsPossessed = false;
    [SyncVar]
    public bool isPossessable = true;
    [SyncVar]
    public bool TriggerIsallowed = true;
    #endregion

    #region Current Object Info variables
    [Header("Current Object Info")]
    [SerializeField]
    private GameObject CurrentGameObject;
    [SyncVar]
    public GameObject objectEntered;
    public Rigidbody currentRB;
    public float drag = 0.1f;
    public VehicleController CurrentVehicleController;
    public ThirdPersonController currentThirdPersonController;
    public AirborneController CurrentAirborneController;
    public Possession CurrentPossession;
    public PlayerHealth HP;
    #endregion

    #region Crane Related Variables
    [Header("Crane Related Variables")]
    public CraneHandle crane;
    public PickUp objectPickupHandler;
    #endregion

    #region Airborne Controller Variables
    [Header("Airborne Controller Variables")]
    public Rigidbody AirborneRB;
    public PickUp HelicopterHOOK;
    public CraneHandle HelicopterRope;
    #endregion

    #region Solider Controller Variables
    [Header("Solider Controller Variables")]
    public Gun gun;
    public SwitchVcam VcamSetting;
    public CinemachineVirtualCamera vcam;
    public Rigidbody SoliderRb;
    #endregion

    #region Network Variables
    [Header("Network Variables")]
    [SerializeField]
    NetworkIdentity identity;
    Authority auth;
    #endregion
    
    //This should just run on Client, No need to inform the server
    public void OnEnable()
    {
        UIHandler = GameObject.Find("UI Manager");
        OverallUI = UIHandler.GetComponent<UIManager>().OverallUI;
        Transform[] dummy = OverallUI.GetComponentsInChildren<Transform>(true);
        if (dummy[1].name == "Panel")
            Message = dummy[1].gameObject;

        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        inputManager.possession = this;
        CurrentGameObject = transform.parent.gameObject;

        identity = netIdentity;
    }

    //This should just run on Client, No need to inform the server
    private void OnDisable()
    {
        inputManager.DisablePossessionFeature();
    }
    
    //[ClientCallback]
    //private void Start()
    //{
    //    //Testing Slider implementation
    //    //endValue = startingValue * 10f;
    //    //minValue = 0;
    //    //MainSlider.maxValue = endValue;
    //}

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        //If current script is active and there is no connection to client than it is possessable
        if (CurrentPossession.isActiveAndEnabled && identity.connectionToClient == null)
        {
            //The object can trigger
            if (TriggerIsallowed)
            {
                Toys CacheEnum;
                
                //If it has no Enum than it is not a possessable object
                if (other.GetComponent<EnumsID>() == null)
                {
                    return;
                }
                CacheEnum = other.GetComponent<EnumsID>().Enum;
                
                //No more triggering
                TriggerIsallowed = false;

                //If the object that touched was a Ghost than turn the possession feature on
                if (CacheEnum == Toys.Ghost)
                {
                    //Store the reference of the entered object
                    objectEntered = other.gameObject;

                    //Turning the message ON for specific client
                    NetworkIdentity network = other.gameObject.GetComponent<NetworkIdentity>();
                    TurnOnMsgAndAssignMovement(network.connectionToClient);

                    //Storing the reference of the Ghost
                    //Ghost = other.gameObject;
                }
                else
                {
                    //Found no Ghost so turn the trigger area on for other clients
                    TriggerIsallowed = true;
                    Debug.Log("Returning! The object that entered trigger was " + other.name +
                        " and has connection to client: " + identity.connectionToClient);
                    return;
                }
            }
            else
            {
                Debug.Log("Someone already in Trigger Area. Trigger is Allowed = " + TriggerIsallowed);
                TriggerIsallowed = true;
                return;
            }
        }
        else
        {
            Debug.Log("Current script is disabled or already owned. Returning!");
            return;
        }
    }

    [Server]
    private void OnTriggerExit(Collider other)
    {
        //If triggering is allowed (means we found no Ghost)
        //checking for connection to client, if yes than it is already possessed
        //if no, than it can still be possessed
        if (TriggerIsallowed || identity.connectionToClient != null)
        {
            return;
        }

        if (CurrentPossession.isActiveAndEnabled)
        {
            //If not possessed
            if (!IsPossessed)
            {
                //Turn on Trigger area for other clients
                TriggerIsallowed = true;


                if (other.gameObject.GetComponent<EnumsID>() != null)
                {
                    //Saving Enum value
                    Toys CacheEnum = other.gameObject.GetComponent<EnumsID>().Enum;
                    
                    //if Ghost..
                    if (CacheEnum == Toys.Ghost)
                    {
                        //Disable current possession script
                        CurrentPossession.enabled = false;
                        
                        //Turning off message for specific clients
                        NetworkIdentity network = other.gameObject.GetComponent<NetworkIdentity>();
                        TurnOffMsgAndAssignMovement(network.connectionToClient);
                        
                        //Nullifying all objects used
                        OverAllCamera = null;
                        objectEntered = null;
                    }
                    else
                    {
                        Debug.Log("Returning! The object that entered trigger was " + other.gameObject.name +
                            " and is local player: " + other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer);
                        return;
                    }
                }
                else
                {
                    Debug.Log("Somehow we got a null object: " + other.gameObject.name + ". Returning!");
                    return;
                }
            }
            else
            {
                Debug.Log("The object is already possessed. Returning!");
                return;
            }
        }
        else
        {
            Debug.Log("Current script is disabled. Returning!");
            return;
        }
    }

    [Client]
    public void Posses(InputAction.CallbackContext context)
    {
        //If it is possessable and trigger area is closed, it means someone is in the area
        if(!TriggerIsallowed)
        {
            //If this object is not possessed and it has no connection to client than...
            if (!IsPossessed && !identity.isOwned)
            {
                //Storing the reference of the Ghost
                Ghost = objectEntered;
                //Turn off the UI
                Message.SetActive(false);

                //Cache the Enum
                Toys CacheEnum = CurrentGameObject.GetComponent<EnumsID>().Enum;

                //If the object to possess is either a Toy Car or Crane
                if (CacheEnum == Toys.ToyCar || CacheEnum == Toys.Crane)
                {
                    //Enable health and damage for that object
                    //HP.enabled = true;
                        
                    //Turn on the relative Vehicle Controller
                    CurrentVehicleController.enabled = true;
                        
                    //Reduce the drag of the object so it can move smoothly
                    currentRB.drag = 0.1f;
                        
                    //Some specific conditions for Crane
                    if (CacheEnum == Toys.Crane)
                    {
                        //Turn on Rope and crane handler
                        crane.enabled = true;
                        
                        //Turn on Object pickup handler
                        objectPickupHandler.enabled = true;
                    }
                        
                    //Special case of snapping a car and solider together
                    if(CacheEnum == Toys.ToyCar && CurrentGameObject.GetComponentInChildren<CarSnapping>() != null && CurrentGameObject.GetComponentInChildren<CarSnapping>().isSnapped)
                    {
                        vcam.Priority += 10;
                        gun.enabled = true;
                    }
                }
                //If the object to possess is a Helicopter
                else if (CacheEnum == Toys.Helicopter)
                {
                    HP.enabled = true;
                    CurrentAirborneController.enabled = true;
                    AirborneRB.useGravity = false;
                    HelicopterHOOK.enabled = true;
                    HelicopterRope.enabled = true;
                }
                //If the object to possess is a Solider
                else if (CacheEnum == Toys.Solider)
                {
                    //HP.enabled = true;
                    gun.enabled = true;
                    //OverAllCamera.gameObject.SetActive(false);
                    VcamSetting.enabled = true;
                    SoliderRb.isKinematic = true;
                }
                //If the object to possess is a Bear
                else if (CacheEnum == Toys.Bear)
                {
                    currentThirdPersonController.enabled = true;
                    //HP.enabled = true;
                }

                //if the object that triggered is not Ground than ...
                if (objectEntered != null && objectEntered.gameObject.name != "Ground")
                {
                    //Look for a third person controller on that object and turn it off
                    objectEntered.gameObject.GetComponent<ThirdPersonController>().enabled = false;
                }

                /*Camera Setup for characters other than soliders
                //if (CacheEnum != Toys.Solider && OverAllCamera != null)
                //{
                //    OverAllCamera.transform.SetParent(CurrentGameObject.transform, false);
                //    CinemachineFreeLook freeLook = OverAllCamera.GetComponent<CinemachineFreeLook>();
                //    freeLook.Follow = CurrentGameObject.transform;
                //    freeLook.LookAt = CurrentGameObject.transform;
                }*/

                //Unchecking variables 
                CmdPossession_Variables();

                if (CacheEnum != Toys.Solider)
                {
                    //Camera Setup
                    var camera = Ghost.transform.GetComponentInChildren<CinemachineFreeLook>(true);
                    //In case the Camera is off
                    if (camera != null && !camera.gameObject.activeSelf)
                    {
                        camera.gameObject.SetActive(true);
                    }
                    //Setting the Camera for the Possessed objects
                    camera.transform.SetParent(CurrentGameObject.transform, false);
                    camera.Follow = CurrentGameObject.transform;
                    camera.LookAt = CurrentGameObject.transform;
                    //Manage Camera
                    cameraManager.ChangeCamera(camera, transform.parent.gameObject);
                }
                else
                {
                    VcamSetting.enabled = true;
                }
                //Turn off Ghost VFX
                CmdTurnOffGhost();
            }
            //If this object is possessed and it has connection to client than...
            else if (IsPossessed && identity.isOwned)
            {
                //Turn on Health
                //HP.enabled = false;

                if (Ghost != null && Ghost.GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                    //Put the Ghost at the position of the Bear
                    Ghost.transform.SetPositionAndRotation(gameObject.transform.position, Quaternion.Euler(Vector3.zero));
                    //Enable its Third Person controller
                    Ghost.GetComponent<ThirdPersonController>().enabled = true;
                    
                    //Setup the Camera for that object
                    var camera = transform.parent.GetComponentInChildren<CinemachineFreeLook>(true);
                    camera.transform.SetParent(Ghost.transform, false);
                    camera.Follow = Ghost.transform;
                    camera.LookAt = Ghost.transform;
                    cameraManager.ChangeCamera(camera, Ghost);

                    //Turn ON Ghost VFX
                    CmdTurnOnGhost();
                }

                Toys CacheEnum = CurrentGameObject.GetComponent<EnumsID>().Enum;

                //Which object are we unpossessing? Turn the controller off for that object else the object will keep on responding to WASD key inputs
                if (CacheEnum == Toys.ToyCar || CacheEnum == Toys.Crane)
                {
                    CurrentVehicleController.enabled = false;
                    //If the object that we unpossessed is still moving than increse drag so its comes to a stop fast
                    if (currentRB.velocity.magnitude > 0)
                    {
                        currentRB.velocity = Vector3.zero;
                        currentRB.drag = 2f;
                    }
                    //If the unpossessed object was a crane then turn of the crane rope handler else it will keep on responding to Arrow Key inputs
                    if (CacheEnum == Toys.Crane)
                    {
                        crane.enabled = false;
                        objectPickupHandler.enabled = false;
                    }
                    if(CacheEnum == Toys.ToyCar && CurrentGameObject.GetComponentInChildren<CarSnapping>() != null && CurrentGameObject.GetComponentInChildren<CarSnapping>().isSnapped)
                    {
                        vcam.Priority -= 10;
                        gun.enabled = false;
                    }
                }
                else if(CacheEnum == Toys.Helicopter)
                {
                    CurrentAirborneController.enabled = false;
                    //If the object that we unpossessed is still moving than increse drag so its comes to a stop fast
                    if (currentRB.velocity.magnitude > 0)
                    {
                        currentRB.drag = 2f;
                    }
                    CurrentGameObject.GetComponent<Rigidbody>().useGravity = true;
                    //OverAllCamera.m_Lens.FieldOfView = 40;
                    HelicopterHOOK.enabled = true;
                    HelicopterRope.enabled = true;
                }
                else if(CacheEnum == Toys.Solider)
                {
                    gun.enabled = false;
                    OverAllCamera.gameObject.SetActive(true);
                    VcamSetting.enabled = false;
                    CurrentGameObject.transform.rotation = Quaternion.Euler(new Vector3(0f,CurrentGameObject.transform.rotation.y ,0f));
                }
                else if(CacheEnum == Toys.Bear)
                {
                    currentThirdPersonController.enabled = false;
                    
                    Rigidbody rigidbody = CurrentGameObject.GetComponent<Rigidbody>();
                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                    
                    CurrentGameObject.GetComponent<Animator>().SetBool("isWalking", false);
                }

                //Unchecking a few variables for next time use
                CmdUnPossession_Variables();
                
                if (identity.isOwned)
                {
                    Debug.Log("Removing authority from the Bear");
                    
                    auth = Ghost.GetComponent<Authority>();
                    
                    auth.CmdRemoveAuthority(identity);
                }
            }
        }
        else
        {
            return;
        }
    }


    /// <summary>
    /// Network functions
    /// </summary>
    [TargetRpc]
    void TurnOnMsgAndAssignMovement(NetworkConnectionToClient network)
    {
        Message.SetActive(true);
        inputManager.EnablePossessionFeature();
    }
    [TargetRpc]
    void TurnOffMsgAndAssignMovement(NetworkConnectionToClient network)
    {
        Message.SetActive(false);
        inputManager.DisablePossessionFeature();
    }

    [Command (requiresAuthority = false)]
    void CmdPossession_Variables()
    {
        RpcPossession_Variables();
    }
    [ClientRpc]
    void RpcPossession_Variables()
    {
        Debug.Log("Setting Possession variables");
        IsPossessed = true;
        isPossessable = false;
        TriggerIsallowed = false;
    }

    [Command(requiresAuthority = false)]
    void CmdUnPossession_Variables()
    {
        RpcUnPossession_Variables();
    }
    [ClientRpc]
    void RpcUnPossession_Variables()
    {
        IsPossessed = false;
        TriggerIsallowed = true;
        isPossessable = true;
    }

    [Command(requiresAuthority = false)]
    void CmdTurnOffGhost()
    {
        RpcTurnOffGhost();
    }
    [ClientRpc]
    void RpcTurnOffGhost()
    {
        Debug.Log("Turning VFX off on client-side");
        //Turn off all objects in Childern so the Ghost still exists but all the children are off
        for (int i = 0; i < objectEntered.transform.childCount; i++)
        {
            objectEntered.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    [Command]
    void CmdTurnOnGhost()
    {
        RpcTurnOnGhost();
    }
    [ClientRpc]
    void RpcTurnOnGhost()
    {
        //Turn off all objects in Childern so the Ghost still exists but all the children are off
        if(Ghost != null)
        {
            for (int i = 0; i < objectEntered.transform.childCount; i++)
            {
                Ghost.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}


public enum Toys
{
    Ghost,
    Bear,
    ToyCar,
    Crane,
    Solider,
    Helicopter,
    AIRobotShooter,
    AIRobotPicker
}