using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float currhealth = 100;
    public const int maxHealth = 100;

    public void PrintHealth(GameObject ui)
    {
        Debug.Log("Currhealth: " + currhealth + "/" + maxHealth);
        ui.SetActive(true);
        ui.GetComponentInChildren<TMP_Text>().text = "Currhealth: " + currhealth + "/" + maxHealth;
    }

    public void Addhealth(int health)
    {
        if(currhealth < maxHealth)
        {
            if(currhealth + health > maxHealth)
            {
                currhealth = maxHealth;
            }
            else
            {
                currhealth += health;
            }
        }
    }

    //If true returned, player still has health
    //If false returned, player's health reached zero
    public bool TakeDamage(float damage)
    {
        if(currhealth - damage < 0)
        {
            currhealth = 0;
            return false;
        }
        currhealth -= damage;
        return true;
    }
}
