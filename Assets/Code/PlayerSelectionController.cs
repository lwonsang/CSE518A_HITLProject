using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionController : MonoBehaviour
{
    public SelectableImage[] imageSlots;

    private KeyCode[] blueKeys = {
        KeyCode.Q, KeyCode.W, KeyCode.E,
        KeyCode.A, KeyCode.S, KeyCode.D,
        KeyCode.Z, KeyCode.X, KeyCode.C
    };

    private KeyCode[] redKeys = {
        KeyCode.U, KeyCode.I, KeyCode.O,
        KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.Comma, KeyCode.Period
    };

    public enum Player { Blue, Red }
    public Player currentPlayer = Player.Blue;

    private bool submitted = false;
    private int currentQuestionIndex = 0;
    public int questionsPerRound = 5;
    void Update()
    {
        if (submitted) return;

        KeyCode[] activeKeys = currentPlayer == Player.Blue ? blueKeys : redKeys;

        for (int i = 0; i < activeKeys.Length; i++)
        {
            if (Input.GetKeyDown(activeKeys[i]))
            {
                imageSlots[i].ToggleSelection();
            }
        }

        if ((currentPlayer == Player.Blue && Input.GetKeyDown(KeyCode.Space)) ||
            (currentPlayer == Player.Red && Input.GetKeyDown(KeyCode.Return)))
        {
            SubmitAnswer();
        }
    }

    void SubmitAnswer()
    {
        List<int> selectedIndices = new List<int>();
        for (int i = 0; i < imageSlots.Length; i++)
        {
            if (imageSlots[i].IsSelected())
            {
                selectedIndices.Add(i);
            }
        }

        Debug.Log($"{currentPlayer} Q{currentQuestionIndex} selected: {string.Join(", ", selectedIndices)}");

        GameManager.Instance.RecordPlayerAnswer(currentPlayer, currentQuestionIndex, selectedIndices);

        currentQuestionIndex++;

        if (currentQuestionIndex >= questionsPerRound)
        {
            submitted = true;
            GameManager.Instance.ForceEndTurn(currentPlayer);
        }
        else
        {
            LoadNextQuestion();
        }
    }
    void LoadNextQuestion()
    {
        submitted = false;

        Debug.Log($"Load question {currentQuestionIndex} for {currentPlayer}");

        foreach (var img in imageSlots)
        {
            img.ResetSelection();
        }
    }
    public void SetPlayer(Player p)
    {
        currentPlayer = p;
        submitted = false;
        currentQuestionIndex = 0;
        LoadNextQuestion();
        //foreach (var img in imageSlots)
        //{
            //img.ResetSelection();
        //}
    }
}
