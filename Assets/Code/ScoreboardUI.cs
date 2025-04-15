using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreboardUI : MonoBehaviour
{
    public TextMeshProUGUI roundNumberText;
    public TextMeshProUGUI winnerText;
    public Button nextButton;
    public Canvas scoreboardCanvas;

    private int currentRound = 1;
    private bool gameOver = false;

    void Start()
    {
        // Set initial UI state
        scoreboardCanvas.gameObject.SetActive(false);
        nextButton.onClick.AddListener(OnNextButtonClick);
        nextButton.gameObject.SetActive(false);
        
        // Display the round number at the start
        UpdateRoundUI();
    }

    void UpdateRoundUI()
    {
        scoreboardCanvas.gameObject.SetActive(true);
        roundNumberText.text = "Round " + currentRound;
    }

    public void ShowFinalScore(string winner, float bluePercentage, float redPercentage)
    {
        scoreboardCanvas.gameObject.SetActive(true);
        // Hide the round number text
        roundNumberText.gameObject.SetActive(false);

        // Show final results
        winnerText.gameObject.SetActive(true);
        winnerText.text = $"{winner} Wins!\n\nBlue: {bluePercentage:F2}%\nRed: {redPercentage:F2}%";
        nextButton.gameObject.SetActive(true);
        gameOver = true; // Mark the game as over
    }

    void OnNextButtonClick()
    {
        if (gameOver)
        {
            SceneManager.LoadScene("Main Menu"); // Load main menu if game is over
        }
        else
        {
            currentRound++;
            UpdateRoundUI();
        }
    }
}
