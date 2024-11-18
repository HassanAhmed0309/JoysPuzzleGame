using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public GameObject Door;
    bool openingDoor = false;
    public float DoorOpenTime = 0f;
    float yval = 0f;
    GameObject saveGameObject = null;
    
    [SerializeField]
    private List<Transform> NewWaypoints;

    [SerializeField]
    private AI AIPicker;

    private void OnTriggerEnter(Collider collision)
    {
        if (gameObject.name == Buttons.ShootableButtonA.ToString() || gameObject.name == Buttons.ShootableButtonB.ToString() || gameObject.name == Buttons.ShootableButtonC.ToString())
        {
            if (Door.name == Doors.PermanentOpenA.ToString() || Door.name == Doors.PermanentOpenB.ToString() || Door.name == Doors.PermanentOpenC.ToString())
            {
                if (collision.gameObject.name == "Bullet(Clone)")
                {
                    Door.transform.localScale = new Vector3(Door.transform.localScale.x, 0.1f, Door.transform.localScale.z);
                }
            }
            else if (Door.name == Doors.TemporaryOpenA.ToString() || Door.name == Doors.TemporaryOpenB.ToString() || Door.name == Doors.TemporaryOpenC.ToString())
            {
                if (collision.gameObject.name == "Bullet(Clone)" && openingDoor == false)
                {
                    yval = Door.transform.localScale.y;
                    openingDoor = true;
                    Door.transform.localScale = new Vector3(Door.transform.localScale.x, Door.transform.localScale.y - yval, Door.transform.localScale.z);
                    StartCoroutine(KeepOpening());
                }
            }
        }
        else if (gameObject.name == Buttons.PlaceableButtonA.ToString() || gameObject.name == Buttons.PlaceableButtonB.ToString() || gameObject.name == Buttons.PlaceableButtonC.ToString())
        {
            saveGameObject = collision.gameObject;
            yval = Door.transform.localScale.y;
            Door.transform.localScale = new Vector3(Door.transform.localScale.x, Door.transform.localScale.y - yval, Door.transform.localScale.z);
            if(NewWaypoints != null)
            {
                foreach (Transform T in NewWaypoints)
                {
                    AIPicker.Waypoints.Add(T);
                }
            }
        }
    }

    IEnumerator KeepOpening()
    {
        yield return new WaitForSeconds(DoorOpenTime);
        Door.transform.localScale = new Vector3(Door.transform.localScale.x, Door.transform.localScale.y + yval, Door.transform.localScale.z);
        openingDoor = false;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (gameObject.name == Buttons.PlaceableButtonA.ToString() || gameObject.name == Buttons.PlaceableButtonB.ToString() || gameObject.name == Buttons.PlaceableButtonC.ToString()) 
        {
            if (collision.gameObject.name == saveGameObject.name)
            {
                Door.transform.localScale = new Vector3(Door.transform.localScale.x, Door.transform.localScale.y + yval, Door.transform.localScale.z);
                if(NewWaypoints != null)
                {
                    for (int i = 0; i < AIPicker.Waypoints.Count; i++)
                    {
                        if (NewWaypoints.Contains(AIPicker.Waypoints[i]))
                        {
                            AIPicker.Waypoints.RemoveAt(i);
                        }
                    }
                }    
                
            }
        }
    }
}

public enum Doors
{
    TemporaryOpenA,
    TemporaryOpenB,
    TemporaryOpenC,
    PermanentOpenA,
    PermanentOpenB,
    PermanentOpenC
}

public enum Buttons
{
    PlaceableButtonA,
    PlaceableButtonB,
    PlaceableButtonC,
    ShootableButtonA,
    ShootableButtonB,
    ShootableButtonC
}
