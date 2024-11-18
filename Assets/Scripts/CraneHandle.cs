using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraneHandle : NetworkBehaviour
{
    public float MaxUpHeight = 1f;
    public float MaxDownHeight = 4f;

    public float currentRopeHeight = 0f;

    public float ropeSpeed = 0.03f;

    public GameObject Hook;

    private float val = 0;

    public GameObject currentGameObject;


    // Start is called before the first frame update
    void Start()
    {
        currentRopeHeight = gameObject.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        currentRopeHeight = gameObject.transform.localScale.y;
        
        Toys CacheEnum = currentGameObject.GetComponent<EnumsID>().Enum;

        if(currentGameObject != null && CacheEnum == Toys.Helicopter)
        {
            //Rope up
            if (Keyboard.current.vKey.isPressed)
            {
                if (currentRopeHeight <= MaxDownHeight && currentRopeHeight >= MaxUpHeight)
                {
                    if (gameObject.transform.localScale.y + ropeSpeed > MaxDownHeight)
                    {
                        val = MaxDownHeight;
                    }
                    else
                    {
                        val = gameObject.transform.localScale.y + ropeSpeed;
                        Hook.transform.SetLocalPositionAndRotation(new Vector3(Hook.transform.localPosition.x, Hook.transform.localPosition.y - ropeSpeed, Hook.transform.localPosition.z), Hook.transform.localRotation);
                    }
                    gameObject.transform.localScale = new Vector3(1f, val, 1f);
                }
            }
            //Rope Down
            if (Keyboard.current.bKey.isPressed)
            {
                if (currentRopeHeight <= MaxDownHeight && currentRopeHeight >= MaxUpHeight)
                {
                    if (gameObject.transform.localScale.y - ropeSpeed < MaxUpHeight)
                    {
                        val = MaxUpHeight;
                    }
                    else
                    {
                        val = gameObject.transform.localScale.y - ropeSpeed;
                        Hook.transform.SetLocalPositionAndRotation(new Vector3(Hook.transform.localPosition.x, Hook.transform.localPosition.y + ropeSpeed, Hook.transform.localPosition.z), Hook.transform.localRotation);
                    }
                    gameObject.transform.localScale = new Vector3(1f, val, 1f);
                }
            }
        }
        else
        {
            //Rope up
            if (Keyboard.current.vKey.isPressed)
            {
                if (currentRopeHeight <= MaxDownHeight && currentRopeHeight >= MaxUpHeight)
                {
                    if (gameObject.transform.localScale.y + ropeSpeed > MaxDownHeight)
                    {
                        val = MaxDownHeight;
                    }
                    else
                    {
                        val = gameObject.transform.localScale.y + ropeSpeed;
                        Hook.transform.SetLocalPositionAndRotation(new Vector3(Hook.transform.localPosition.x, Hook.transform.localPosition.y - ropeSpeed, Hook.transform.localPosition.z), Hook.transform.localRotation);
                    }
                    gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, val, gameObject.transform.localScale.z);
                }
            }
            //Rope Down
            if (Keyboard.current.bKey.isPressed)
            {
                if (currentRopeHeight <= MaxDownHeight && currentRopeHeight >= MaxUpHeight)
                {
                    if (gameObject.transform.localScale.y - ropeSpeed < MaxUpHeight)
                    {
                        val = MaxUpHeight;
                    }
                    else
                    {
                        val = gameObject.transform.localScale.y - ropeSpeed;
                        Hook.transform.SetLocalPositionAndRotation(new Vector3(Hook.transform.localPosition.x, Hook.transform.localPosition.y + ropeSpeed, Hook.transform.localPosition.z), Hook.transform.localRotation);
                    }
                    gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, val, gameObject.transform.localScale.z);
                }
            }
        }

    }

    public void ResetRope(Toys enim)
    {     
        if(enim == Toys.Helicopter)
        {
            //gameObject.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            //Hook.transform.SetLocalPositionAndRotation(new Vector3(6.61f, -6.77f, 0.7f), Hook.transform.rotation);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            Hook.transform.SetLocalPositionAndRotation(new Vector3(6.61f, -6.77f, 0.7f), Hook.transform.rotation);
        }
    }
}
