using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;


public class CheckForPossession : NetworkBehaviour
{
    public Possession currentPossession;

    private void OnTriggerEnter(Collider other)
    {
        var EnumOrNot = other.gameObject.GetComponent<EnumsID>();
        if (EnumOrNot != null && EnumOrNot.Enum == Toys.Ghost)
        {
            var thisPossesion = gameObject.GetComponent<Possession>();
            if (!thisPossesion.enabled)
                thisPossesion.enabled = true;   
        }
    }
}
