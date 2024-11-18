using Cinemachine.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    #region Variables
    #region Public Variables
    public List<Transform> Waypoints;
    public List<Transform> DuplicateWaypoints;

    GameObject LocationofBox;

    public float Threshold = 20f;
    public int time = 30;
    int timecounter = 0;

    public int CurrWaypointIndex = 0;
   
    public float Radius;
    [Range(0, 360)]
    public float Angle;
    
    public Gun Turrent;
    public AI thisai;
    
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public LayerMask Pickable;
    public LayerMask NotPickable;

    public bool CanSeePlayer;
    public bool StartChasing = false;
    public bool MovedBox = false;
    public bool AlmostReachedDestination = false;
    #endregion

    #region Private Variables
    [SerializeField]
    private NavMeshAgent Agent;
    private NavMeshPath path;
    private GameObject TempGameObject = null;

    private bool FoundPossessedObject = false;

    private int ChangedWaypoint;
    #endregion
    #endregion

    #region Functions
    #region Unity Calls
    private void Start()
    {
        path = new NavMeshPath();
        for (int i = 0; i < Waypoints.Count; i++)
        {
            DuplicateWaypoints.Add(Waypoints[i]);
        }
        StartCoroutine(FOVRoutine());
    }
    private void Update()
    {
        if(thisai.isActiveAndEnabled && thisai.name == "AIRobotShooter")
        {   
            if(TempGameObject != null)
            {
                float distance = Vector3.Distance(transform.position, TempGameObject.transform.position);
                if (distance < 20f && !Agent.isStopped)
                {
                    Agent.isStopped = true;
                    Vector3 directionToTarget = TempGameObject.transform.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                }
                else if(distance > 20f && Agent.isStopped)
                {
                    Agent.isStopped = false;
                }
            }    
            if (StartChasing && timecounter > time)
            {
                Turrent.ShootBasedOnObject(TempGameObject.gameObject);
                timecounter = 0;
            }
            timecounter++;
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        StartChasing = false;
        Turrent.enabled = false;
        Agent.enabled = false;
    }
    #endregion

    #region FOV and Movement
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
            Walking();
        }
    }
    private void FieldOfViewCheck()
    {
        Toys CacheEnum = GetComponent<EnumsID>().Enum;
        if (CacheEnum == Toys.AIRobotShooter)
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, Radius, targetMask);

            if (rangeChecks.Length != 0)
            {
                RaycastHit hit;

                foreach (var rangecheck in rangeChecks)
                {
                    Transform target = rangecheck.transform;
                    Vector3 directionToTarget = (target.position - transform.position).normalized;

                    // If object in 180 degree Angle
                    if (Mathf.Abs(Vector3.Angle(transform.forward, directionToTarget)) < Angle / 2)
                    {
                        //Find the distance to target
                        float distanceToTarget = Vector3.Distance(transform.position, target.position);

                        bool HitOrNot = Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, targetMask);

                        if (hit.transform != null)
                                Debug.Log("Hit: " + hit.transform.name);

                        //if (hit.transform != null && hit.transform.GetComponentInChildren<Possession>().IsPossessed)
                        //{
                        //    HitOrNot = true;
                        //    target = hit.transform;
                        //}

                        //If the ray hits something other than Obstruction masks
                        if (HitOrNot)
                        {
                            //We just saw the object
                            CanSeePlayer = true;
                            //But is it even possessable?
                            Possession possession = target.GetComponentInChildren<Possession>();
                            if (possession != null && !FoundPossessedObject)
                            {
                                 
                                //If so, then output the information of that object.
                                Debug.Log("I can see u!  " + target.gameObject.name + "  " + possession.isActiveAndEnabled + "   " +
                                    possession.IsPossessed);

                                //if the possession script is active and the object in view is possessed, The Object is Possessed
                                if (possession.isActiveAndEnabled && possession.IsPossessed)
                                {
                                    //Get the index of the waypoint the AI was moving towards
                                    ChangedWaypoint = CurrWaypointIndex;
                                    //Set the object in view to be the waypoint so the AI keeps on changing
                                    Waypoints[CurrWaypointIndex] = target.transform;
                                    //Start the chase
                                    StartChasing = true;
                                    //Enable shooting towards the target object
                                    Turrent.enabled = true;
                                    //Save the target object for later use
                                    TempGameObject = target.gameObject;
                                    //Turrent.ShootBasedOnObject(TempGameObject.gameObject);

                                    //Found possessed object
                                    FoundPossessedObject = true;
                                }
                            }
                        }

                    }
                }
            }
            else if (CanSeePlayer)
                CanSeePlayer = false;
            //if the object was found possessed
            if (FoundPossessedObject && TempGameObject != null)
            {
                if (TempGameObject.GetComponentInChildren<Possession>() != null && !TempGameObject.GetComponentInChildren<Possession>().isActiveAndEnabled)
                {
                    CanSeePlayer = false;
                    //Set the waypoint back
                    Waypoints[ChangedWaypoint] = DuplicateWaypoints[ChangedWaypoint];
                    //Stop chasing
                    StartChasing = false;
                    //Stop the Turrent
                    Turrent.enabled = false;
                    //
                    TempGameObject = null;
                    FoundPossessedObject = false;
                    //rangeChecks = null;
                    if(Agent.isStopped)
                    {
                        Agent.isStopped = false;
                    }
                }
            }
        }
        else if (CacheEnum == Toys.AIRobotPicker)
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, Radius, Pickable);
            if (rangeChecks.Length != 0)
            {
                Debug.Log(rangeChecks.Length);
                Transform target;
                target = rangeChecks[0].transform;
                if(rangeChecks.Length > 1)
                {
                    Debug.Log("Lengths two!");
                    Collider highestPriority = rangeChecks[0];
                    foreach(var rangeCheck in rangeChecks)
                    {
                        if(rangeCheck.name != TempGameObject.name)
                        {
                            if (TempGameObject.GetComponent<PickableBlock>().priority > rangeCheck.GetComponent<PickableBlock>().priority)
                            {
                                highestPriority = rangeCheck;
                            }    
                        }
                    }
                    target = highestPriority.transform;
                }
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                // If object in 180 degree Angle
                if (Mathf.Abs(Vector3.Angle(transform.forward, directionToTarget)) < Angle / 2)
                {
                    RaycastHit hit;
                    //Find the distance to target 
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if(!Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, NotPickable))
                    {
                        if(hit.transform != null)
                            Debug.Log(hit.transform.name + ": " + hit.distance);
                    }

                    //If the ray hits something other than Not Pickable objects
                    if (!Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, NotPickable) && !AlmostReachedDestination)
                    {
                        if(hit.transform != null)
                            Debug.Log("Found first block ");
                        TempGameObject = target.gameObject;
                        PickableBlock block = TempGameObject.GetComponent<PickableBlock>();
                        block.IsMoved();
                        if(block.HasMoved)
                        {
                            MovedBox = true;
                        }
                    }
                    if(!Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, NotPickable) && AlmostReachedDestination)
                    {
                        if(target.name != transform.GetComponentInChildren<PickableBlock>().gameObject.name)
                        {
                            if (hit.transform != null)
                                Debug.Log("Found another block! ");
                            PickableBlock pickable = TempGameObject.GetComponent<PickableBlock>();
                            PickableBlock pickableBlock = target.GetComponent<PickableBlock>();
                            pickableBlock.IsMoved();
                            if (pickable.priority > pickableBlock.priority && pickableBlock.HasMoved)
                            {
                                DropBox(TempGameObject);
                                TempGameObject = target.gameObject;
                                AlmostReachedDestination = false;
                            }
                        }
                        
                    }
                    if (MovedBox && !AlmostReachedDestination)
                    {
                        Debug.Log("Box needs to be moved");
                        PickUpBox(TempGameObject);
                        ChangedWaypoint = CurrWaypointIndex;
                        LocationofBox = new GameObject();
                        LocationofBox.transform.position = TempGameObject.GetComponent<PickableBlock>().SavedPosition;
                        Waypoints[CurrWaypointIndex] = LocationofBox.transform;
                        AlmostReachedDestination = true;
                    }
                    if(LocationofBox != null && Vector3.Distance(transform.position, LocationofBox.transform.position) < Threshold && AlmostReachedDestination)
                    {
                        AlmostReachedDestination = false;
                        DropBox(TempGameObject);
                        TempGameObject.transform.position = TempGameObject.GetComponent<PickableBlock>().SavedPosition;
                        Waypoints[ChangedWaypoint] = DuplicateWaypoints[ChangedWaypoint];
                        MovedBox = false;
                        LocationofBox = null;
                    }
                }      
            }
        }
    }

    private void DropBox(GameObject tempGameObject)
    {
        Rigidbody player = tempGameObject.GetComponent<Rigidbody>();
        Transform playerTransform = tempGameObject.transform;
        playerTransform.SetParent(null);
        player.useGravity = true;
        player.constraints = RigidbodyConstraints.None;
    }

    private void PickUpBox(GameObject PickedUpObject)
    {
        Rigidbody player = PickedUpObject.GetComponent<Rigidbody>();
        Transform playerTransform = PickedUpObject.transform;
        player.useGravity = false;
        player.constraints = RigidbodyConstraints.FreezeAll;
        playerTransform.SetParent(transform);
        playerTransform.SetLocalPositionAndRotation(new Vector3(0f, 0.0053f, 0.017f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
    }

    private void Walking()
    {
        if (Waypoints.Count <= 0)
        {
            return;
        }
        //CurrWaypointIndex = Val;
        float distanceToWaypoint = Vector3.Distance(Waypoints[CurrWaypointIndex].position, transform.position);

        if (distanceToWaypoint <= 3)
        {
            //Val = Random.Range(0, Waypoints.Count);
            CurrWaypointIndex = (CurrWaypointIndex + 1) % Waypoints.Count;

            Agent.CalculatePath(Waypoints[CurrWaypointIndex].transform.position, path);
            while(path.status != NavMeshPathStatus.PathComplete)
            {
                CurrWaypointIndex = (CurrWaypointIndex + 1) % Waypoints.Count;
                Agent.CalculatePath(Waypoints[CurrWaypointIndex].transform.position, path);
            }
        }

        Agent.SetDestination(Waypoints[CurrWaypointIndex].position);
    }
    #endregion
    #endregion
}
public enum Priority
{
    High,
    Medium,
    Low
}