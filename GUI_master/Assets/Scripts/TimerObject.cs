using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerObject : MonoBehaviour
{

    float timer = 15*60;

    bool timerPaused = true;

    public string startingString = "";

    [Header("GUI")]
    public Text timerText;

    private void Update()
    {
        if (!timerPaused)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                timerText.text = startingString + ":\n" + timer;
            }
            else
            {
                StopTimer();
            }
        }
    }

    public void StartTimer(float initialValue)
    {
        if (initialValue < 0)
            return;

        timer = initialValue;
    }

    public void OverrideTimer(float timerValue)
    {
        timer = timerValue;
    }

    public void PauseTimer()
    {
        timerPaused = true;
    }

    public void ResumeTimer()
    {
        timerPaused = false;
    }

    public void StopTimer()
    {
        timerText.text = "";
        timerPaused = true;
    }

    /*public void SetTimer(float val)
    {
        if (val < 0)
            return;
    }*/

}
