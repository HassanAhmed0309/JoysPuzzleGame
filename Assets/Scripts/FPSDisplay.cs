using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI FPS_text;

    public float poolingTime = 1f;
    private float time;
    private float frameCount;
    void Update()
    {
        time += Time.deltaTime;

        frameCount++;

        if (time >= poolingTime) 
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            FPS_text.text = frameRate.ToString() + " FPS";

            time -= poolingTime;
            frameCount = 0;
        }
    }
}
