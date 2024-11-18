using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public void ChangeCamera(Cinemachine.CinemachineFreeLook freeLook, GameObject ToChangeFor)
    {
        Toys ChangeEnum = ToChangeFor.GetComponent<EnumsID>().Enum;

        if (ChangeEnum == Toys.Ghost)
        {
            freeLook.m_YAxis.m_MaxSpeed = 0.5f;
            
            freeLook.m_XAxis.m_MaxSpeed = 100;

            freeLook.m_Orbits[0].m_Height = 9f;
            freeLook.m_Orbits[0].m_Radius = 17f;

            freeLook.m_Orbits[1].m_Height = 5f;
            freeLook.m_Orbits[1].m_Radius = 15f;
            
            freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 0f, 0f);

            freeLook.m_Orbits[2].m_Height = -5f;
            freeLook.m_Orbits[2].m_Radius = 15f;
        }
        else if(ChangeEnum == Toys.ToyCar)
        {
            freeLook.m_Lens.FieldOfView = 40;

            freeLook.m_Orbits[0].m_Height = 14f;
            freeLook.m_Orbits[0].m_Radius = 12f;

            freeLook.m_Orbits[1].m_Height = 5f;
            freeLook.m_Orbits[1].m_Radius = 18f;

            freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 0f, 0f);

            freeLook.m_Orbits[2].m_Height = -0.89f;
            freeLook.m_Orbits[2].m_Radius = 12f;
        }
        else if (ChangeEnum == Toys.Crane)
        {
            freeLook.m_YAxis.m_Recentering.m_RecenteringTime = 10;
            freeLook.m_YAxis.m_Recentering.m_WaitTime = 5;

            freeLook.m_XAxis.m_Recentering.m_RecenteringTime = 10;
            freeLook.m_XAxis.m_Recentering.m_WaitTime = 5;

            freeLook.m_Orbits[0].m_Height = 20f;
            freeLook.m_Orbits[0].m_Radius = 15f;

            freeLook.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 15f, 0f);

            freeLook.m_Orbits[1].m_Height = 5f;
            freeLook.m_Orbits[1].m_Radius = 20f;

            freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 6f, 0f);

            freeLook.m_Orbits[2].m_Height = -0.89f;
            freeLook.m_Orbits[2].m_Radius = 20f;

            freeLook.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 5f, 0f);

        }
        else if(ChangeEnum == Toys.Helicopter)
        {
            freeLook.m_Lens.FieldOfView = 40;

            freeLook.m_Orbits[0].m_Height = 14f;
            freeLook.m_Orbits[0].m_Radius = 12f;

            freeLook.m_Orbits[1].m_Height = 6f;
            freeLook.m_Orbits[1].m_Radius = 27f;

            freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 0f, 0f);

            freeLook.m_Orbits[2].m_Height = -0.89f;
            freeLook.m_Orbits[2].m_Radius = 12f;
        }
        else if (ChangeEnum == Toys.Solider)
        {
            freeLook.m_YAxis.m_Recentering.m_RecenteringTime = 10;
            freeLook.m_YAxis.m_Recentering.m_WaitTime = 5;

            freeLook.m_XAxis.m_Recentering.m_RecenteringTime = 10;
            freeLook.m_XAxis.m_Recentering.m_WaitTime = 5;

            freeLook.m_Orbits[0].m_Height = 20f;
            freeLook.m_Orbits[0].m_Radius = 17f;

            freeLook.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 5f, 0f);

            freeLook.m_Orbits[1].m_Height = 16.5f;
            freeLook.m_Orbits[1].m_Radius = 19f;

            freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 5f, 0f);

            freeLook.m_Orbits[2].m_Height = -6f;
            freeLook.m_Orbits[2].m_Radius = 15f;

            freeLook.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 5f, 0f);

        }
        else if(ChangeEnum == Toys.Bear)
        {
            freeLook.m_SplineCurvature = 0.2f;

            freeLook.m_YAxis.m_MaxSpeed = 0.5f;

            freeLook.m_XAxis.m_MaxSpeed = 100;

            freeLook.m_Orbits[0].m_Height = 18f;
            freeLook.m_Orbits[0].m_Radius = 20f;

            freeLook.m_Orbits[1].m_Height = 13f;
            freeLook.m_Orbits[1].m_Radius = 20f;

            freeLook.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 5f, 0f);
            freeLook.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = new Vector3(0f, 10f, 0f);

            freeLook.m_Orbits[2].m_Height = 1f;
            freeLook.m_Orbits[2].m_Radius = 12f;
        }
    }
}
