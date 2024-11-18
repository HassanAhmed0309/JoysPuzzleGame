using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnumsID : MonoBehaviour
{
    public Toys Enum;
    public string ID;

    string Ghost = Toys.Ghost.ToString();
    string Bear = Toys.Bear.ToString();
    string ToyCar = Toys.ToyCar.ToString();
    string Solider= Toys.Solider.ToString();
    string Helicopter = Toys.Helicopter.ToString();
    string Crane = Toys.Crane.ToString();
    string AIRobotPicker = Toys.AIRobotPicker.ToString();
    string AIRobotShooter = Toys.AIRobotShooter.ToString();

    // Start is called before the first frame update
    void Start()
    {
        //Automatically assigning enums to values
        if(gameObject.name.Contains(ToyCar))
        {
            Enum = Toys.ToyCar;
            ID = Toys.ToyCar.ToString() + (int)Toys.ToyCar;
        }
        else if (gameObject.name.Contains(Ghost))
        {
            Enum = Toys.Ghost;
            ID = Toys.Ghost.ToString() + (int)Toys.Ghost;
        }
        else if(gameObject.name.Contains(Bear))
        {
            Enum = Toys.Bear;
            ID = Toys.Bear.ToString() + (int)Toys.Bear;
        }
        else if (gameObject.name.Contains(Helicopter))
        {
            Enum = Toys.Helicopter;
            ID = Toys.Helicopter.ToString() + (int)Toys.Helicopter;
        }
        else if (gameObject.name.Contains(Solider))
        {
            Enum = Toys.Solider;
            ID = Toys.Solider.ToString() + (int)Toys.Solider;
        }
        else if (gameObject.name.Contains(Crane))
        {
            Enum = Toys.Crane;
            ID = Toys.Crane.ToString() + (int)Toys.Crane;
        }
        else if (gameObject.name.Contains(AIRobotShooter))
        {
            Enum = Toys.AIRobotShooter;
            ID = Toys.AIRobotShooter.ToString() + (int)Toys.AIRobotShooter;
        }
        else if (gameObject.name.Contains(AIRobotPicker))
        {
            Enum = Toys.AIRobotPicker;
            ID = Toys.AIRobotPicker.ToString() + (int)Toys.AIRobotPicker;
        }
    }
}
