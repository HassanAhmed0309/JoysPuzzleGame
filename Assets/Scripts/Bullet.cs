using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    [SerializeField]
    private float speed = 50f;
    
    public float life = 2;

    [SerializeField]
    Rigidbody rb;
    public Vector3 target { get; set; }
    public string Name { get; set; }



    private void OnEnable()
    {
        Destroy(gameObject, life);
    }

    private void Update()
    {

    }
    
    [Server]
    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Collided with: " + other.gameObject.name);
        if (other.gameObject.name == "button")
        {
            other.gameObject.transform.parent.transform.GetComponent<AI>().enabled = false;
        }
    }
}