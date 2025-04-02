using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    // Start is called before the first frame update
    void Start()
    {
        fullWidth = timerBarFill.sizeDelta.x;
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
        }
        else
        {
            momentumBarBg.redCorrect += correctSelections;
            momentumBarBg.redIncorrect += incorrectSelections;
            momentumBarBg.redTotal += totalCorrect;
        }
        momentumBarBg.UpdateMomentum();
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
        while (currentRound <= maxRounds)
        {
            currentState = GameState.RoundIntro;
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
            MomentumBar_BG momentumBarBg = FindObjectOfType<MomentumBar_BG>();
            scoresText.text = $"Blue: {Mathf.Round(momentumBarBg.blueScore * 100.0f) * 0.01f}% | Red: {Mathf.Round(momentumBarBg.redScore * 100.0f) * 0.01f}%";
            StartCoroutine(Countdown(3f));
            yield return WaitWithSkip(3f);
            skipWaiting = false;

            currentRound++;
        }

        currentState = GameState.GameOver;
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


}
