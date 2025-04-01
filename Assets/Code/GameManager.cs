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

            if (currentRound < maxRounds)
            {
                if (blueGoesFirst)
                {
                    currentState = GameState.BlueTurn;
                    ShowOnly(panelBlueTurn);
                    Debug.Log("Blue Player Turn");
                    panelBlueTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Blue);
                    StartCoroutine(Countdown(15f));
                    yield return WaitWithSkip(15f);
                    skipWaiting = false;

                    currentState = GameState.RedTurn;
                    ShowOnly(panelRedTurn);
                    Debug.Log("Red Player Turn");
                    panelRedTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Red);
                    StartCoroutine(Countdown(15f));
                    yield return WaitWithSkip(15f);
                    skipWaiting = false;
                }
                else
                {
                    currentState = GameState.RedTurn;
                    ShowOnly(panelRedTurn);
                    Debug.Log("Red Player Turn");
                    panelRedTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Red);
                    StartCoroutine(Countdown(15f));
                    yield return WaitWithSkip(15f);
                    skipWaiting = false;

                    currentState = GameState.BlueTurn;
                    ShowOnly(panelBlueTurn);
                    Debug.Log("Blue Player Turn");
                    panelBlueTurn.GetComponentInChildren<PlayerSelectionController>().SetPlayer(PlayerSelectionController.Player.Blue);
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
                Debug.Log("Final Round - Both Players");
                StartCoroutine(Countdown(30f));
                yield return WaitWithSkip(30f);
                skipWaiting = false;
            }

            currentState = GameState.RoundResult;
            ShowOnly(panelRoundResult);
            scoresText.text = $"Blue: XX% | Red: XX%";
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
}
