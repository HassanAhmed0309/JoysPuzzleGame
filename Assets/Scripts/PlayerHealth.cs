using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{ 
    static HealthManager healthManager;

    public PlayerHealth P;

    [SyncVar] public GameObject ui;

    [ClientCallback]
    private void Start()
    {
        healthManager = new HealthManager();
    }
    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if(P.isActiveAndEnabled)
        {
            if (collision.gameObject.name == "Bullet(Clone)")
            {
                healthManager.TakeDamage(.5f);
                healthManager.PrintHealth(ui);
            }
        }
    }
}
