using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    public GameObject UI;
    public GameObject Quit;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<EnumsID>() != null && other.gameObject.GetComponent<EnumsID>().Enum == Toys.Bear)
        {
            UI.SetActive(true);
            UI.GetComponentInChildren<TMP_Text>().text = "Game End";
            other.gameObject.GetComponent<ThirdPersonController>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            other.gameObject.GetComponent<Animator>().SetBool("isWalking", false);
            Quit.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Works");
        Application.Quit();
    }    
}
