using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PickUp : NetworkBehaviour
{
    [SerializeField]
    Possession referencePossession;
    InputManager inputManager;
    [SerializeField]
    private GameObject Hook;
    [SerializeField] 
    private GameObject HookTarget;
    [SerializeField]
    private Rigidbody CraneRb;

    public Collider CollidedWith;

    GameObject MessagePanel;
    TMP_Text MessageText;

    public PickUp pickup;
    
    public bool isPicked = false;
    bool isHeld = false;

    public float HookObjectDistance = 2f;

    [SerializeField]
    CraneHandle craneHandle;

    public GameObject currGameObject;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        inputManager.pickUp = this;
        inputManager.EnablePickupFeature();
        MessagePanel = referencePossession.Message;
        MessageText = MessagePanel.GetComponent<TMP_Text>();
    }

    public void Pickup(InputAction.CallbackContext context)
    {
        if(!isPicked && !isHeld && CollidedWith != null)
        {
            if(CollidedWith.gameObject.name == "Solider" && CollidedWith.gameObject.GetComponent<CarSnapping>().isSnapped)
            {

            }
            else
            {
                CollidedWith.gameObject.transform.SetParent(Hook.transform);
                Possession possession = CollidedWith.gameObject.GetComponentInChildren<Possession>();
                if (possession != null)
                {
                    possession.isPossessable = false;
                }
                Rigidbody rb = CollidedWith.gameObject.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.mass = 1f;
                CollidedWith.GetComponent<Collider>().isTrigger = true;

                MessagePanel.SetActive(false);
                isPicked = true;
                CollidedWith.transform.SetLocalPositionAndRotation(new Vector3(-6.5f, 5.7f, 0f), Quaternion.Euler(Vector3.zero));

                isHeld = true;
            }
        }
        else if (isHeld)
        {
            isPicked = false;
            CollidedWith.gameObject.transform.parent = null;
            if(CollidedWith.transform != null && CollidedWith.gameObject.name == "Solider")            
                CollidedWith.GetComponent<CarSnapping>().CanBeSnapped = true;


            Rigidbody rb = CollidedWith.gameObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.mass = 1000f;
            CollidedWith.GetComponent<Collider>().isTrigger = false;

            Possession cachepossession = CollidedWith.gameObject.GetComponentInChildren<Possession>();
            if (cachepossession != null)
                cachepossession.isPossessable = true;
            isHeld = false;
            CollidedWith = null;

        }
    } 

    private void Update()
    {
        if(isPicked)
        {
            Toys Helicopter = currGameObject.GetComponent<EnumsID>().Enum;
            if (currGameObject != null && Helicopter == Toys.Helicopter)
            {
                Debug.Log("Collided with: " + currGameObject.name);
                CollidedWith.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero)); 
            }
            else
                CollidedWith.transform.SetLocalPositionAndRotation(new Vector3(0f, .5f, 0f), Quaternion.Euler(Vector3.zero));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickup.isActiveAndEnabled)
        {
            Debug.Log(other.gameObject.name);
            if(!isPicked)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Toys") || other.gameObject.layer == LayerMask.NameToLayer("Blockers"))
                {
                    CollidedWith = other;
                    MessagePanel.SetActive(true);
                    MessageText.text = "Press P to pick up " + other.name;
                }
            }
            if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                craneHandle.ResetRope(Toys.Helicopter);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (pickup.isActiveAndEnabled)
        {
            Toys Solider;
            MessagePanel.SetActive(false);
            if (other.GetComponent<EnumsID>() != null)
            {
                Solider = other.GetComponent<EnumsID>().Enum;
                if (Solider == Toys.Solider)
                {
                    craneHandle.enabled = true;
                }
            }
        }
    }

    private void OnDisable()
    {
        if(inputManager != null)
            inputManager.DisablePickupFeature();
    }
}
