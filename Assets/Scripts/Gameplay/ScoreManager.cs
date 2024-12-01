using System;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static event Action<ScoreManager> OnScoreManagerInit;
    public UnityEvent<int> OnScoreChanged;
    [SerializeField] private int totalScore;
    [SerializeField] private int highestScore;

    [Header("Score Values")]
    [SerializeField] public int scorePerEnemy;
    [SerializeField] private int scorePerCoin;
    [SerializeField] private int scorePerPowerUp;

    private void OnEnable()
    {
        Character.OnCharacterInitialized += SetupCharacterListeners;
        GameManager.OnEnemyKilled += IncreaseScore;
    }

    private void OnDisable()
    {
        Character.OnCharacterInitialized -= SetupCharacterListeners;
        GameManager.OnEnemyKilled -= IncreaseScore;
    }

    private void SetupCharacterListeners(Character character)
    {
        if (character is Player player)
        {
            if (player.healthValue != null)
            {
                player.healthValue.OnDeath.AddListener(RegisterScore);
            }
            else
            {
                Debug.LogError("ScoreManager: Player's healthValue is not initialized.");
            }
        }
    }


    private void Start()
    {
        OnScoreManagerInit?.Invoke(this);
    }

    private void RegisterScore()
    {
        // NEW HIGH SCORE!
        if (totalScore > highestScore)
        {
            highestScore = totalScore;
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
