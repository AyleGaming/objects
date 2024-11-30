using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<int> OnHighestScoreChange;
    [SerializeField] private int totalScore;
    [SerializeField] private int highestScore;

    [Header("Score Values")]
    [SerializeField] public int scorePerEnemy;
    [SerializeField] private int scorePerCoin;
    [SerializeField] private int scorePerPowerUp;

    [SerializeField] private List<ScoreData> allScores = new List<ScoreData>();

    [SerializeField] private ScoreData latestScore;

    private void Start()
    {
        Player playerObject = FindObjectOfType<Player>();
        playerObject.healthValue.OnDeath.AddListener(RegisterScore);
        
        highestScore = PlayerPrefs.GetInt("HighScore");
        
        // At start of game
        // try to convert back into score data
        string latestScoreInJson = PlayerPrefs.GetString("LatestScore");

    }

    private void RegisterScore()
    {
        //create an object filled with information
        latestScore = new ScoreData("RDA", totalScore);

        // convert the object (class) to a string in json format
        string latestScoreInJson = JsonUtility.ToJson(latestScore);

        // save to playerprefs
        PlayerPrefs.SetString("LatestScore", latestScoreInJson);
             
        // NEW HIGH SCORE!
        if (totalScore > highestScore)
        {
            highestScore = totalScore;
            PlayerPrefs.SetInt("HighScore", highestScore);
        }
    }

    public void IncreaseScore(ScoreType action)
    {
        switch (action)
        {
            case ScoreType.EnemyKilled:
                totalScore += scorePerEnemy;
                break;

            case ScoreType.CoinCollected:
                totalScore += scorePerCoin;
                break;

            case ScoreType.PowerUpCollected:
                totalScore += scorePerPowerUp;
                break;

        }
        OnScoreChanged.Invoke(totalScore);
    }
}
