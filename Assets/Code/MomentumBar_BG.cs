using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MomentumBar_BG : MonoBehaviour
{
    public Image redFill;
    public Image blueFill;
    public float redCorrect = 0; 
    public float blueCorrect = 0;
    public float redIncorrect = 0;
    public float blueIncorrect = 0;
    public float redTotal = 1;
    public float blueTotal = 1;
    public float redScore;
    public float blueScore;
    [Range(-1f, 1f)] private float momentum;

    void Update()
    {
        float normalizedMomentum = (momentum + 1f) / 2f; // Convert to 0 to 1 scale
        redFill.fillAmount = 1f - normalizedMomentum;
        blueFill.fillAmount = normalizedMomentum;
        Debug.Log("Momentum: " + momentum + " | Red Fill: " + redFill.fillAmount + " | Blue Fill: " + blueFill.fillAmount);
    }
    public void UpdateMomentum()
    {
        redScore = Mathf.Max(0, (redCorrect - redIncorrect) / redTotal) * 100;
        blueScore = Mathf.Max(0, (blueCorrect - blueIncorrect) / blueTotal) * 100;
        float totalScore = blueScore + redScore;
        if (totalScore == 0) return;
        momentum = blueScore /  totalScore;
        Debug.Log("redCorrect " +  redCorrect + " blueCorrect " + blueCorrect + " redIncorrect "  + redIncorrect + " blueIncorrect " + blueIncorrect + " redTotal " + redTotal + " blueTotal " + blueTotal);
        Debug.Log("momentum " + momentum + " | Red Score: " + redScore + " | Blue Score: " + blueScore);
    }
}
