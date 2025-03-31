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
    IEnumerator Countdown(float duration)
    {
        float timeLeft = duration;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            float t = Mathf.Clamp01(timeLeft / duration);
            timerBarFill.sizeDelta = new Vector2(fullWidth * t, timerBarFill.sizeDelta.y);
            yield return null;
        }

        timerBarFill.sizeDelta = new Vector2(0f, timerBarFill.sizeDelta.y);
    }
    IEnumerator GameFlow()
    {
        while (currentRound <= maxRounds)
        {
            currentState = GameState.RoundIntro;
            ShowOnly(panelRoundIntro);
            roundNumberText.text = $"Round {currentRound}";
            StartCoroutine(Countdown(2f));
            yield return new WaitForSeconds(2f);

            if (currentRound < maxRounds)
            {
                if (blueGoesFirst)
                {
                    currentState = GameState.BlueTurn;
                    ShowOnly(panelBlueTurn);
                    Debug.Log("Blue Player Turn");
                    StartCoroutine(Countdown(15f));
                    yield return new WaitForSeconds(15f);

                    currentState = GameState.RedTurn;
                    ShowOnly(panelRedTurn);
                    Debug.Log("Red Player Turn");
                    StartCoroutine(Countdown(15f));
                    yield return new WaitForSeconds(15f);
                }
                else
                {
                    currentState = GameState.RedTurn;
                    ShowOnly(panelRedTurn);
                    Debug.Log("Red Player Turn");
                    StartCoroutine(Countdown(15f));
                    yield return new WaitForSeconds(15f);

                    currentState = GameState.BlueTurn;
                    ShowOnly(panelBlueTurn);
                    Debug.Log("Blue Player Turn");
                    StartCoroutine(Countdown(15f));
                    yield return new WaitForSeconds(15f);
                }

                blueGoesFirst = !blueGoesFirst;
            }
            else
            {
                currentState = GameState.FinalRound;
                ShowOnly(panelFinalRound);
                Debug.Log("Final Round - Both Players");
                StartCoroutine(Countdown(30f));
                yield return new WaitForSeconds(30f);
            }

            currentState = GameState.RoundResult;
            ShowOnly(panelRoundResult);
            scoresText.text = $"Blue: XX% | Red: XX%";
            StartCoroutine(Countdown(3f));
            yield return new WaitForSeconds(3f);

            currentRound++;
        }

        currentState = GameState.GameOver;
        Debug.Log("Game Over. Show final results here.");
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
