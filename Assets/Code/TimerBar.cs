using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    public Image fillBar;
    public float timerDuration = 15f;
    private float timer;
    private bool isTiming = false;

    void Start()
    {
        timer = timerDuration;
        //fillBar.fillAmount = 0f;
    }
    
    void Update()
    {
        if (isTiming)
        {
            // Decrease the timer
            timer -= Time.deltaTime;

            // Calculate the fill amount based on the remaining time
            fillBar.fillAmount = 1 - (timer / timerDuration); // The bar fills up as the timer counts down

            // Ensure the timer doesn't go below 0
            if (timer <= 0)
            {
                timer = 0;
                isTiming = false; // Stop the timer when it reaches zero
            }
        }
    }

    // Method to start the timer (can be called externally if needed)
    public void StartTimer()
    {
        isTiming = true;
    }

    // Method to reset the timer (can be called externally if needed)
    public void ResetTimer()
    {
        timer = timerDuration;
        fillBar.fillAmount = 0f;
        isTiming = false;
    }
}
