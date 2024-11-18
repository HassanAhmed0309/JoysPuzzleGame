using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject OverallUI;
    [SerializeField]
    GameObject Eventsystem;

    // Start is called before the first frame update
    void Start()
    {
        GameObject cloned;
        cloned = Instantiate(OverallUI);
        cloned.name = OverallUI.name;
        cloned.SetActive(true); 

        //cloned = Instantiate(Eventsystem);
        //cloned.name = Eventsystem.name;
        //cloned.SetActive(true);
    }
}
