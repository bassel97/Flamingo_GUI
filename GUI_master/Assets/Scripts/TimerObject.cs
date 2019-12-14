using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerObject : MonoBehaviour
{

    float timer = 1500;

    bool timerPaused = false;

    [Header("GUI")]
    public Text timerText;

    private void Update()
    {
        if (!timerPaused)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                timerText.text = "Timer:\n" + timer;
            }
        }
    }

    public void StartTimer(float initialValue)
    {
        if (initialValue < 0)
            return;

        timer = initialValue / 1000;
    }

    public void PauseTimer()
    {
        timerPaused = true;
    }

    public void ResumeTimer()
    {
        timerPaused = false;
    }

    /*public void SetTimer(float val)
    {
        if (val < 0)
            return;
    }*/

}
