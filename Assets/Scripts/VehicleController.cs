using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class VehicleController : NetworkBehaviour
{
    private InputManager inputManager;

    public Possession CarPossession;

    public WheelColliders colliders;
    public WheelMeshes meshes;

    public float gasInput;
    public float steeringInput;

    public float brakepower;
    public float brakeInput;
    private float slipAngle;

    public float motorPower;
    public float speed;
    public float maxSpeed = 40f;
    public float currentDrag;
    public AnimationCurve steeringCurve;

    public bool IsToppledOver = false;

    public bool KeepBodyStatic = false;

    public Rigidbody CurrentRB;

    public GameObject CarStillToppled;

    public GameObject TopPart;

    public float CraneRotationSpeed = .5f;

    public Cinemachine.CinemachineFreeLook CinemachineCamera;
    
    public CameraManager CameraManager;

    [SerializeField]
    [Range(0f, 4f)] float lerptime;
    // Start is called before the first frame update
    private void OnEnable()
    {
        var Enum = GetComponent<EnumsID>().Enum;
        if (Enum == Toys.ToyCar || Enum == Toys.Crane)
        {
            var clientID = CarPossession.Ghost.GetComponent<NetworkIdentity>();
            var CarID = netIdentity;
            if (!isOwned && clientID.isLocalPlayer)
            {
                Authority auth = CarPossession.Ghost.GetComponent<Authority>();
                auth.CmdAssignAuthority(clientID, CarID);
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
        
        currentDrag = CurrentRB.drag;

        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        inputManager.EnableVehicleMovement();
        CameraManager = new CameraManager();
        CinemachineCamera = GameManager.Instance.localCamera;
        CameraManager.ChangeCamera(CinemachineCamera, this.gameObject);
        if (isOwned) { CurrentRB.isKinematic = false; }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!KeepBodyStatic)
        {
            CheckInput();
            ApplyingMotor();
            ApplySteering();
            ApplyBreak();
            WheelsPosition();
        }
        

        Toys CacheEnum = GetComponent<EnumsID>().Enum;
        
        if(CacheEnum == Toys.Crane)
        {
            if (inputManager.VehicleRotateAntiClockwiseMovement())
            {
                TopPart.transform.Rotate(-1 * transform.up * CraneRotationSpeed * Time.deltaTime);
            }
            else if (inputManager.VehicleRotateClockwiseMovement())
            {
                TopPart.transform.Rotate(transform.up * CraneRotationSpeed * Time.deltaTime);
            }
        }
    }

    void ApplySteering()
    {
        float steeringAngle = steeringInput*steeringCurve.Evaluate(speed);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void CheckInput()
    {
        if(Keyboard.current.wKey.isPressed)
        {
            gasInput = 1;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            gasInput = -1;
        }
        else
        {
            gasInput = 0;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            steeringInput = -1;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            steeringInput = 1;
        }
        else
        {
            steeringInput = 0;
        }
        slipAngle = Vector3.Angle(transform.forward, CurrentRB.velocity - transform.forward);
        if(slipAngle<120f)
        {
            if (gasInput < 0)
            {
                brakeInput = Mathf.Abs(gasInput);
                gasInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }

    }

    private void ApplyBreak()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakepower *0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakepower *0.7f;

        colliders.BRWheel.brakeTorque = brakeInput * brakepower * 0.7f;
        colliders.BLWheel.brakeTorque = brakeInput * brakepower * 0.7f;

    }

    void ApplyingMotor()
    {
        colliders.BRWheel.motorTorque = motorPower * gasInput;
        colliders.BLWheel.motorTorque = motorPower * gasInput;
        colliders.FLWheel.motorTorque = motorPower * gasInput;
        colliders.FRWheel.motorTorque = motorPower * gasInput;

    }

    void WheelsPosition()
    {
        UpdateWheel(colliders.FLWheel, meshes.FLWheel);
        UpdateWheel(colliders.FRWheel, meshes.FRWheel);
        UpdateWheel(colliders.BLWheel, meshes.BLWheel);
        UpdateWheel(colliders.BRWheel, meshes.BRWheel);
    }

    void UpdateWheel(WheelCollider coll, GameObject wheelMesh)
    {
        Quaternion quat;
        Vector3 pos;
        coll.GetWorldPose(out pos, out quat);
        wheelMesh.transform.position =  pos;
        wheelMesh.transform.rotation = quat;
        //Quaternion.Lerp(wheelMesh.transform.rotation, quat, lerptime)
    }

    IEnumerator toppled()
    {

        yield return new WaitForSeconds(3f);
        if(CarStillToppled != null)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name != "Ground")
        {
            if(collision.gameObject.name == "Solider" && collision.gameObject.GetComponent<CarSnapping>().isSnapped)
            {

            }
            else
            {
                CurrentRB.velocity = Vector3.zero;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            CarStillToppled = collision.gameObject;
            StartCoroutine(toppled());
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        CarStillToppled = null;
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider BRWheel;
    public WheelCollider BLWheel;
}

[System.Serializable]
public class WheelMeshes
{
    public GameObject FRWheel;
    public GameObject FLWheel;
    public GameObject BRWheel;
    public GameObject BLWheel;
}