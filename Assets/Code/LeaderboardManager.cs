using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LeaderboardManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerScore
    {
        public string playerName;
        public float correctAnswers;
        public float incorrectAnswers;
        public float totalAnswers;

        public float TotalScore
        {
            get { return Mathf.Max(0, correctAnswers / (correctAnswers+incorrectAnswers)) * 100; }
        }

        public PlayerScore(string name, float correctAnswers, float incorrectAnswers, float totalAnswers)
        {
            this.playerName = name;
            this.correctAnswers = correctAnswers;
            this.incorrectAnswers = incorrectAnswers;
            this.totalAnswers = totalAnswers;
        }
    }
    [System.Serializable]
    public class PlayerScoreListWrapper
    {
        public List<PlayerScore> playerScores;
    }

    public List<PlayerScore> playerScores = new List<PlayerScore>();

    public void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(new PlayerScoreListWrapper { playerScores = playerScores }, true);
        //use Application.persistentDataPath for builds instead
        File.WriteAllText(Application.dataPath + "/Leaderboard Scores/leaderboard.json", json);
        Debug.Log("Leaderboard saved to: " + Application.dataPath + "Leaderboard Scores/leaderboard.json");
    }
}
