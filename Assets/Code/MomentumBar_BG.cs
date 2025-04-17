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
   public float redTotal = 0;
   public float blueTotal = 0;
   public float redScore;
   public float blueScore;
   public float totalScore;
   public float redBonus = 0;
   public float blueBonus = 0;
   [Range(-1f, 1f)] public float momentum;
   public float maxMomentum = 10f;


   void Update()
   {
       if (blueTotal <= 1f && redTotal <= 1f)
       {
           redFill.fillAmount = 0.5f;
           blueFill.fillAmount = 0.5f;
           return;
       }
      
       // redFill.fillAmount = 1f - momentum;
       // blueFill.fillAmount = momentum;


       //Debug.Log("Momentum: " + momentum + " | Red Fill: " + redFill.fillAmount + " | Blue Fill: " + blueFill.fillAmount);
   }
   public void UpdateMomentum()
   {
       float redMissed = redTotal - redCorrect;
       float blueMissed = blueTotal - blueCorrect;
       blueScore = blueCorrect + redIncorrect + redMissed + blueBonus;
       redScore = redCorrect + blueIncorrect + blueMissed + redBonus;
       totalScore = blueScore - redScore;
       momentum = (totalScore + maxMomentum) / (2*maxMomentum);
       if (blueTotal <= 1f && redTotal <= 1f)
       {
           redFill.fillAmount = 0.5f;
           blueFill.fillAmount = 0.5f;
           return;
       }
      
       redFill.fillAmount = 1f - momentum;
       blueFill.fillAmount = momentum;


       // Debug.Log("Total Scores: redCorrect " +  redCorrect + " blueCorrect " + blueCorrect + " redIncorrect "  + redIncorrect + " blueIncorrect " + blueIncorrect + " redTotal " + redTotal + " blueTotal " + blueTotal);
       // Debug.Log("Total Scores: momentum " + momentum + " | total Score: " + totalScore + " | Red Score: " + redScore + " | Blue Score: " + blueScore);
   }


   public void DetermineMaxMomentum(int roundNumber){
       maxMomentum = 30;
       if (roundNumber == 5){
            maxMomentum = 15;
       }
       Debug.Log("max momentum" + maxMomentum);
   }  
}


