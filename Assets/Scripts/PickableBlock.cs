using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableBlock : NetworkBehaviour
{
    [SyncVar]
    public bool HasMoved = false;

    public float Threshold = 10f;

    [SyncVar]
    public Vector3 SavedPosition;

    [SyncVar]
    public bool Picked = false;

    [SyncVar]
    public Priority priority;
    
    float Distance = 0f;

    Rigidbody BoxRb;
    // Start is called before the first frame update
    void Start()
    {
        BoxRb = GetComponent<Rigidbody>();
        BoxRb.inertiaTensor = Vector3.zero;
        SavedPosition = transform.position;
    }

    public void IsMoved()
    {
        if(!Picked)
        {
            Distance = Vector3.Distance(SavedPosition, transform.position);
            if (!HasMoved && Distance > Threshold)
            {
                HasMoved = true;

            }
            else
            {
                HasMoved = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Hook")
            Picked = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Hook")
            Picked = false;
    }
}
