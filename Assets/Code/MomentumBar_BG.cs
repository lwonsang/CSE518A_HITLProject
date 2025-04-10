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
        if (blueTotal <= 1f || redTotal <= 1f)
        {
            redFill.fillAmount = 0.5f;
            blueFill.fillAmount = 0.5f;
            return;
        }
        
        redFill.fillAmount = 1f - momentum;
        blueFill.fillAmount = momentum;

        //Debug.Log("Momentum: " + momentum + " | Red Fill: " + redFill.fillAmount + " | Blue Fill: " + blueFill.fillAmount);
    }
    public void UpdateMomentum()
    {
        LeaderboardManager.PlayerScore red = new LeaderboardManager.PlayerScore("Red Team", redCorrect, redIncorrect, redTotal);
        redScore = red.TotalScore;
        LeaderboardManager.PlayerScore blue = new LeaderboardManager.PlayerScore("Blue Team", blueCorrect, blueIncorrect, blueTotal);
        blueScore = blue.TotalScore;
        float totalScore = blueScore + redScore;
        if (totalScore == 0) return;
        momentum = blueScore /  totalScore;
        // float bluePoints = blueCorrect + redIncorrect;
        // //float redPoints = redCorrect + blueIncorrect;
        // float totalScore = blueTotal + redTotal;
        // momentum = bluePoints  / totalScore;
        Debug.Log("redCorrect " +  redCorrect + " blueCorrect " + blueCorrect + " redIncorrect "  + redIncorrect + " blueIncorrect " + blueIncorrect + " redTotal " + redTotal + " blueTotal " + blueTotal);
        Debug.Log("momentum " + momentum + " | total Score: " + totalScore + " | Red Score: " + redScore + " | Blue Score: " + blueScore);
    }
}
