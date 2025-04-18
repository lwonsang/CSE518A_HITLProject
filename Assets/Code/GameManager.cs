using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        RoundIntro,
        BlueTurn,
        RedTurn,
        RoundResult,
        FinalRound,
        GameOver
    }

    [Header("Panels")]
    public GameObject panelRoundIntro;
    public GameObject panelBlueTurn;
    public GameObject panelRedTurn;
    public GameObject panelRoundResult;

    [Header("Text Elements")]
    public TMP_Text roundNumberText;
    public TMP_Text scoresText;

    private int currentRound = 1;
    private int maxRounds = 5;
    private bool blueGoesFirst = true;

    private GameState currentState;
    [Header("Timer")]
    public RectTransform timerBarFill;

    private float fullWidth;
    [Header("Fifth Round")]
    public GameObject panelFinalRound;
       public static GameManager Instance;
   private bool skipWaiting = false;
   private bool blueSubmitted = false;
   private bool redSubmitted = false;
   private bool gameEndedEarly = false;


   public QuestionGenerator questionGenerator;


   [Header("Question UI")]
   //public TMP_Text questionLabelText;
   public TMP_Text blueLabelText;
   public TMP_Text redLabelText;
   public TMP_Text finalBlueLabelText;
   public TMP_Text finalRedLabelText;
   public List<Image> blueImages;
   public List<Image> redImages;
   public List<Image> finalBlueImages;
   public List<Image> finalRedImages;
   private Question currentQuestion;
   private List<Question> blueQuestions = new List<Question>();
   private List<Question> redQuestions  = new List<Question>();


   public float redCorrectSelections = 0;
   public float redIncorrectSelections = 0;
   public float redTotalCorrect = 0;
   public float blueCorrectSelections = 0;
   public float blueIncorrectSelections = 0;
   public float blueTotalCorrect = 0;


   public float redCorrect = 0;
   public float redIncorrect = 0;
   public float redTotal = 0;
   public float blueCorrect = 0;
   public float blueIncorrect = 0;
   public float blueTotal = 0;


   public string winner;


   public List<LeaderboardManager.PlayerScore> playerScores;
   // Start is called before the first frame update
   void Start()
   {
       fullWidth = timerBarFill.sizeDelta.x;
       gameEndedEarly = false;
       StartCoroutine(GameFlow());
   }


   // Update is called once per frame
   void Update()
   {
      
   }
   void Awake()
   {
       Instance = this;
   }
   public void OnPlayerFinished(PlayerSelectionController.Player player)
   {
       Debug.Log($"{player} finished answering.");
   }
   public void RecordPlayerAnswer(PlayerSelectionController.Player player, int questionIndex, List<int> selectedIndices)
   {
       List<Question> playerQuestions = (player == PlayerSelectionController.Player.Blue) ? blueQuestions : redQuestions;
       if (questionIndex < 0 || questionIndex >= playerQuestions.Count) return;
      
       Question question = playerQuestions[questionIndex];
       HashSet<int> correctIndices = question.correctIndices;
      
       int correctSelections = 0;
       foreach (int index in selectedIndices)
       {
           if (correctIndices.Contains(index))
           {
               correctSelections++;
           }
       }


       int totalCorrect = correctIndices.Count;
       int incorrectSelections = selectedIndices.Count - correctSelections;
      
       MomentumBar_BG momentumBarBg = FindObjectOfType<MomentumBar_BG>();
       if (player == PlayerSelectionController.Player.Blue)
       {
           momentumBarBg.blueCorrect += correctSelections;
           momentumBarBg.blueIncorrect += incorrectSelections;
           momentumBarBg.blueTotal += totalCorrect;
           blueCorrectSelections += correctSelections;
           blueIncorrectSelections += incorrectSelections;
           blueTotalCorrect += totalCorrect;
           if(incorrectSelections == 0 && totalCorrect - correctSelections == 0){
               momentumBarBg.blueBonus += currentRound;
           }
       }
       else
       {
           momentumBarBg.redCorrect += correctSelections;
           momentumBarBg.redIncorrect += incorrectSelections;
           momentumBarBg.redTotal += totalCorrect;
           redCorrectSelections += correctSelections;
           redIncorrectSelections += incorrectSelections;
           redTotalCorrect += totalCorrect;
           if(incorrectSelections == 0 && totalCorrect - correctSelections == 0){
               momentumBarBg.redBonus += currentRound;
           }
       }
      
        if(currentRound == maxRounds){
           momentumBarBg.UpdateMomentum();
       }
       //momentumBarBg.UpdateMomentum();
       CheckForGameEnd(momentumBarBg);
   }
   IEnumerator Countdown(float duration)
   {
       float timeLeft = duration;


       while (timeLeft > 0f)
       {
           if (skipWaiting) break;
           timeLeft -= Time.deltaTime;
           float t = Mathf.Clamp01(timeLeft / duration);
           timerBarFill.sizeDelta = new Vector2(fullWidth * t, timerBarFill.sizeDelta.y);
           yield return null;
       }


       timerBarFill.sizeDelta = new Vector2(0f, timerBarFill.sizeDelta.y);
   }
   IEnumerator WaitWithSkip(float seconds)
   {
       float timeLeft = seconds;
       skipWaiting = false;


       while (timeLeft > 0f)
       {
           if (skipWaiting) break;
           timeLeft -= Time.deltaTime;
           yield return null;
       }
   }
   IEnumerator GameFlow()
   {
       MomentumBar_BG momentumBarBg = FindObjectOfType<MomentumBar_BG>();
       while (currentRound <= maxRounds)
       {
           currentState = GameState.RoundIntro;
           momentumBarBg.DetermineMaxMomentum(currentRound);
            /*
           momentumBarBg.redCorrect = 0;
           momentumBarBg.blueCorrect = 0;
           momentumBarBg.redIncorrect = 0;
           momentumBarBg.blueIncorrect = 0;
           momentumBarBg.redTotal = 0;
           momentumBarBg.blueTotal = 0;
           momentumBarBg.redBonus = 0;
           momentumBarBg.blueBonus = 0;
            */
           ShowOnly(panelRoundIntro);
           roundNumberText.text = $"Round {currentRound}";
           StartCoroutine(Countdown(2f));
           yield return WaitWithSkip(2f);
           skipWaiting = false;
           int questionsThisRound = (currentRound == maxRounds) ? 10 : 5;
           blueQuestions.Clear();
           redQuestions.Clear();
           for (int i = 0; i < questionsThisRound; i++)
           {
               blueQuestions.Add(questionGenerator.GenerateQuestion());
               redQuestions.Add(questionGenerator.GenerateQuestion());
           }


           if (currentRound < maxRounds)
           {
               if (blueGoesFirst)
               {
                   currentState = GameState.BlueTurn;
                   ShowOnly(panelBlueTurn);
                   //DisplayQuestion();
                   Debug.Log("Blue Player Turn");
                   panelBlueTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Blue, blueQuestions);
                   StartCoroutine(Countdown(15f));
                   yield return WaitWithSkip(15f);
                   skipWaiting = false;
                    if(gameEndedEarly) {
                        yield break;
                    }


                   currentState = GameState.RedTurn;
                   ShowOnly(panelRedTurn);
                   //DisplayQuestion();
                   Debug.Log("Red Player Turn");
                   panelRedTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Red, redQuestions);
                   StartCoroutine(Countdown(15f));
                   yield return WaitWithSkip(15f);
                   skipWaiting = false;
               }
               else
               {
                   currentState = GameState.RedTurn;
                   ShowOnly(panelRedTurn);
                   //DisplayQuestion();
                   Debug.Log("Red Player Turn");
                   panelRedTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Red, redQuestions);
                   StartCoroutine(Countdown(15f));
                   yield return WaitWithSkip(15f);
                   skipWaiting = false;
                    if(gameEndedEarly) {
                        yield break;
                    }

                   currentState = GameState.BlueTurn;
                   ShowOnly(panelBlueTurn);
                   //DisplayQuestion();
                   Debug.Log("Blue Player Turn");
                   panelBlueTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Blue, blueQuestions);
                   StartCoroutine(Countdown(15f));
                   yield return WaitWithSkip(15f);
                   skipWaiting = false;
               }


               blueGoesFirst = !blueGoesFirst;
           }
           else
           {
               blueSubmitted = false;
               redSubmitted = false;
               currentState = GameState.FinalRound;
               ShowOnly(panelFinalRound);
               //DisplayQuestion();
               Debug.Log("Final Round - Both Players");
               panelFinalRound.GetComponentsInChildren<PlayerSelectionController>()[0].SetPlayer(PlayerSelectionController.Player.Blue, blueQuestions);
               panelFinalRound.GetComponentsInChildren<PlayerSelectionController>()[1].SetPlayer(PlayerSelectionController.Player.Red, redQuestions);
               StartCoroutine(Countdown(30f));
               yield return WaitWithSkip(30f);
               skipWaiting = false;
           }


           currentState = GameState.RoundResult;
           ShowOnly(panelRoundResult);
           momentumBarBg.UpdateMomentum();
           float redRoundScore = Mathf.Max(0, (redCorrectSelections - redIncorrectSelections) / redTotalCorrect) * 100;
           float blueRoundScore = Mathf.Max(0, (blueCorrectSelections - blueIncorrectSelections) / blueTotalCorrect) * 100;
           scoresText.text = $"Round {currentRound} Scores:\nBlue: {Mathf.Round(blueRoundScore * 100) * 0.01}% | Red: {Mathf.Round(redRoundScore * 100) * 0.01}%";
           Debug.Log("Red Score for Round " + currentRound + ": " + redRoundScore + " | Red Correct Selections: " + redCorrectSelections + ", Red Incorrect Selections: " + redIncorrectSelections + ", Red Total Correct: " + redTotalCorrect + ", Red Bonus Points: " + momentumBarBg.redBonus);
           Debug.Log("Blue Score for Round " + currentRound + ": " + blueRoundScore + " | Blue Correct Selections: " + blueCorrectSelections + ", Blue Incorrect Selections: " + blueIncorrectSelections + ", Blue Total Correct: " + blueTotalCorrect + ", Blue Bonus Points: " + momentumBarBg.blueBonus);


           redCorrect += redCorrectSelections;
           redIncorrect += redIncorrectSelections;
           redTotal += redTotalCorrect;
           blueCorrect += blueCorrectSelections;
           blueIncorrect += blueIncorrectSelections;
           blueTotal += blueTotalCorrect;
           Debug.Log("Total Scores: redCorrect " +  redCorrect + " blueCorrect " + blueCorrect + " redIncorrect "  + redIncorrect + " blueIncorrect " + blueIncorrect + " redTotal " + redTotal + " blueTotal " + blueTotal);
           Debug.Log("Total Scores: momentum " + momentumBarBg.momentum + " | total Score: " + momentumBarBg.totalScore + " | Red Score: " + momentumBarBg.redScore + " | Blue Score: " + momentumBarBg.blueScore);
           
           redCorrectSelections = 0;
           redIncorrectSelections = 0;
           redTotalCorrect = 0;
           blueCorrectSelections = 0;
           blueIncorrectSelections = 0;
           blueTotalCorrect = 0;
           CheckForGameEnd(momentumBarBg);
           if(gameEndedEarly) {
            // GameEnded();
            yield break;
           }
           else{
            StartCoroutine(Countdown(3f));
            yield return WaitWithSkip(3f);
           }
           skipWaiting = false;


           currentRound++;
       }


       GameEnded();
       Debug.Log("Game Over. Show final results here.");
   }
   public void ForceEndTurn(PlayerSelectionController.Player who)
   {
       //StopAllCoroutines();
       //timerBarFill.sizeDelta = new Vector2(0f, timerBarFill.sizeDelta.y);
       if (who == PlayerSelectionController.Player.Blue)
           blueSubmitted = true;
       else if (who == PlayerSelectionController.Player.Red)
           redSubmitted = true;


       if (currentRound == maxRounds)
       {
           if (blueSubmitted && redSubmitted)
           {
               skipWaiting = true;
           }
       }
       else
       {
           skipWaiting = true;
       }
       //StartCoroutine(ContinueAfterTurn(player));
   }


   IEnumerator ContinueAfterTurn(PlayerSelectionController.Player player)
   {
       yield return new WaitForSeconds(0.5f);


       OnPlayerFinished(player);
   }
  
   void ShowOnly(GameObject targetPanel)
   {
       panelRoundIntro.SetActive(false);
       panelBlueTurn.SetActive(false);
       panelRedTurn.SetActive(false);
       panelRoundResult.SetActive(false);
       panelFinalRound.SetActive(false);
       if (targetPanel != null)
           targetPanel.SetActive(true);
   }


   public void DisplayQuestion()
   {
       currentQuestion = questionGenerator.GenerateQuestion();


       if (currentState == GameState.BlueTurn)
       {
           blueLabelText.text = "Select all that match: " + currentQuestion.label;
           for (int i = 0; i < 9; i++)
           {
               if (i < currentQuestion.images.Count)
               {
                   blueImages[i].sprite = currentQuestion.images[i];
                   blueImages[i].gameObject.SetActive(true);
               }
               else
               {
                   blueImages[i].gameObject.SetActive(false);
               }
           }
       }
       else if (currentState == GameState.RedTurn)
       {
           redLabelText.text = "Select all that match: " + currentQuestion.label;
           for (int i = 0; i < 9; i++)
           {
               if (i < currentQuestion.images.Count)
               {
                   redImages[i].sprite = currentQuestion.images[i];
                   redImages[i].gameObject.SetActive(true);
               }
               else
               {
                   redImages[i].gameObject.SetActive(false);
               }
           }
       }
       else if (currentState == GameState.FinalRound)
       {
           finalBlueLabelText.text = "Select all that match: " + currentQuestion.label;
           finalRedLabelText.text = "Select all that match: " + currentQuestion.label;


           for (int i = 0; i < 9; i++)
           {
               if (i < currentQuestion.images.Count)
               {
                   finalBlueImages[i].sprite = currentQuestion.images[i];
                   finalBlueImages[i].gameObject.SetActive(true);


                   finalRedImages[i].sprite = currentQuestion.images[i];
                   finalRedImages[i].gameObject.SetActive(true);
               }
               else
               {
                   finalBlueImages[i].gameObject.SetActive(false);
                   finalRedImages[i].gameObject.SetActive(false);
               }
           }
       }
   }


   public void GameEnded(){
       currentState = GameState.GameOver;
       ScoreboardUI scoreboard = FindAnyObjectByType<ScoreboardUI>();
      
       LeaderboardManager leaderboardManager = FindObjectOfType<LeaderboardManager>();
       LeaderboardManager.PlayerScore redScore = new LeaderboardManager.PlayerScore("Red Team", redCorrect, redIncorrect, redTotal);
       LeaderboardManager.PlayerScore blueScore = new LeaderboardManager.PlayerScore("Blue Team", blueCorrect, blueIncorrect, blueTotal);
       leaderboardManager.SaveLeaderboard(redScore);
       leaderboardManager.SaveLeaderboard(blueScore);
       if(redScore.accuracy > blueScore.accuracy){
           winner = "Red Team";
       }
       else{
           winner = "Blue Team";
       }
       scoreboard.ShowFinalScore(winner, blueScore.accuracy, redScore.accuracy);
   }


   public void CheckForGameEnd(MomentumBar_BG momentumBar_BG){
       if(momentumBar_BG.redTotal > 0 && momentumBar_BG.blueTotal > 0 && (momentumBar_BG.totalScore >= momentumBar_BG.maxMomentum || momentumBar_BG.totalScore <= -momentumBar_BG.maxMomentum)){
           gameEndedEarly = true;
           currentState = GameState.GameOver;
            ScoreboardUI scoreboard = FindAnyObjectByType<ScoreboardUI>();
            redCorrect += redCorrectSelections;
            redIncorrect += redIncorrectSelections;
            redTotal += redTotalCorrect;
            blueCorrect += blueCorrectSelections;
            blueIncorrect += blueIncorrectSelections;
            blueTotal += blueTotalCorrect;
            Debug.Log("Total Scores: redCorrect " +  redCorrect + " blueCorrect " + blueCorrect + " redIncorrect "  + redIncorrect + " blueIncorrect " + blueIncorrect + " redTotal " + redTotal + " blueTotal " + blueTotal);
            
            LeaderboardManager leaderboardManager = FindObjectOfType<LeaderboardManager>();
            LeaderboardManager.PlayerScore redScore = new LeaderboardManager.PlayerScore("Red Team", redCorrect, redIncorrect, redTotal);
            LeaderboardManager.PlayerScore blueScore = new LeaderboardManager.PlayerScore("Blue Team", blueCorrect, blueIncorrect, blueTotal);
            leaderboardManager.SaveLeaderboard(redScore);
            leaderboardManager.SaveLeaderboard(blueScore);
            if(momentumBar_BG.totalScore < 0){
                winner = "Momentum: Red Team";
            }
            else{
                winner = "Momentum: Blue Team";
            }
            scoreboard.ShowFinalScore(winner, blueScore.accuracy, redScore.accuracy);
       }
      
   }


}




