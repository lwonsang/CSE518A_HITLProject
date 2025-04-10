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
        public float accuracy;

        public float TotalScore
        {
            get { return Mathf.Max(0, correctAnswers / totalAnswers) * 100; }
        }

        public PlayerScore(string name, float correctAnswers, float incorrectAnswers, float totalAnswers)
        {
            this.playerName = name;
            this.correctAnswers = correctAnswers;
            this.incorrectAnswers = incorrectAnswers;
            this.totalAnswers = totalAnswers;
            this.accuracy = TotalScore;
        }
    }
    [System.Serializable]
    public class PlayerScoreListWrapper
    {
        public List<PlayerScore> playerScores;
    }

    public List<PlayerScore> playerScores = new List<PlayerScore>();

    public void SaveLeaderboard(PlayerScore newScore)
    {
        PlayerScoreListWrapper wrapper = new PlayerScoreListWrapper();
        string filePath = Application.dataPath + "/Leaderboard Scores/leaderboard.json";
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            wrapper = JsonUtility.FromJson<PlayerScoreListWrapper>(existingJson);
        }
        else
        {
            wrapper.playerScores = new List<PlayerScore>();
        }
        wrapper.playerScores.Add(newScore);
        wrapper.playerScores.Sort((score1, score2) => score2.accuracy.CompareTo(score1.accuracy));
        string json = JsonUtility.ToJson(wrapper, true);
        //use Application.persistentDataPath for builds instead
        File.WriteAllText(filePath, json);
        Debug.Log("Leaderboard saved to: " + Application.dataPath + "Leaderboard Scores/leaderboard.json");
        
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // Refresh for Unity Editor
        #endif
    }
}
