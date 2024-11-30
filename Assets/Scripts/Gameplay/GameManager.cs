using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private ScoreManager scoreManager;
   
    public UnityEvent OnGameStart;
    public UnityEvent OnGameOver;
    public UnityEvent<float> OnUltimateStatusChanged;
    public UnityEvent<int> OnLevelUpChanged;
    public UnityEvent<float> OnLevelPercentStatusChanged;

    [SerializeField] private PickUp pickUpPrefab;
    [SerializeField] private PickUp[] pickUpPrefabs;
    [SerializeField] private GameObject pickUpEffect;
       
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform[] spawnPointsArray;
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private List<Enemy> listOfAllEnemiesAlive;

    [SerializeField] private int totalEnemiesKilled = 0;
    [SerializeField] private int gameLevel = 1;

    private int maxEnemies = 10;
    private float minSpawnDelay = 1f; // Minimum delay for spawning
    private float maxSpawnDelay = 3f; // Maximum delay for spawning
    private float spawnDelayMultiplier = 0.8f; // Multiplier to reduce delay every 10 levels

    private float levelMultiplier = 1.2f; // increase amount of enemies required per level
    
    private int killsForUltimate = 5;
    private int killsSinceLastUltimate = 0;

    private void GameOver()
    {
        OnGameOver.Invoke();
        StopAllCoroutines();
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        scoreManager = GetComponent<ScoreManager>();

        FindObjectOfType<Player>().healthValue.OnDeath.AddListener(GameOver);
        FindObjectOfType<Player>().OnUltimateStatusAvailable.AddListener(EnablePlayerUltimate);

        StartCoroutine(SpawnWaveOfEnemies());
        SpawnEnemy();
    }

    private Enemy SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPointsArray.Length);
        Transform randomSpawnPoint = spawnPointsArray[randomIndex];

        int enemiesToSpawnByRound = EnemiesToSpawn();
        Enemy randomEnemy = enemyPrefabs[Random.Range(0, enemiesToSpawnByRound)];
        Enemy enemyClone = Instantiate(randomEnemy, randomSpawnPoint.position, randomSpawnPoint.rotation);
        listOfAllEnemiesAlive.Add(enemyClone);
        return enemyClone;
    }

    private int EnemiesToSpawn()
    {
        int enemyCount;
        if (gameLevel >= enemyPrefabs.Length)
        {
            enemyCount = enemyPrefabs.Length;
        }
        else
        {
            enemyCount = ((gameLevel - enemyPrefabs.Length) + enemyPrefabs.Length);
        }
        return enemyCount;
    }

    private PickUp SpawnPickUp(Enemy enemyToBeRemoved)
    {
        PickUp randomPrefab = pickUpPrefabs[Random.Range(0, pickUpPrefabs.Length)];
        PickUp pickUpClone = Instantiate(randomPrefab, enemyToBeRemoved.transform.position, Quaternion.identity);

        return pickUpClone;
    }

    public void RemoveEnemyFromList(Enemy enemyToBeRemoved)
    {
        totalEnemiesKilled++;
        killsSinceLastUltimate++;
        scoreManager.IncreaseScore(ScoreType.EnemyKilled);
        if(totalEnemiesKilled % 6 == 0)
        {
            SpawnPickUp(enemyToBeRemoved);
        }

        // Determine level up

        int requiredEnemies = GetEnemiesRequiredForLevelUp();

        if (totalEnemiesKilled >= requiredEnemies)
        {
            LevelUp();
        }

        if (totalEnemiesKilled % killsForUltimate == 0)
        {
            EnablePlayerUltimate(true);
        }
        listOfAllEnemiesAlive.Remove(enemyToBeRemoved);

        if(killsSinceLastUltimate > killsForUltimate)
        {
            OnUltimateStatusChanged.Invoke(100f);
        }
        else
        {
            float killsToUltimatePercent = (float)killsSinceLastUltimate / killsForUltimate * 100;
            OnUltimateStatusChanged.Invoke(killsToUltimatePercent);
        }

        float killPercentToLevel = (((float)totalEnemiesKilled % 10) / 10) * 100;

        Debug.Log(killPercentToLevel);

        OnLevelPercentStatusChanged.Invoke(killPercentToLevel);

        //  Debug.Log("Pause Game");
        //  Time.timeScale = 0;
    }

    private void EnablePlayerUltimate(bool status)
    {
        Player.Instance.SetUltimateAvailable(status);
 
        // Ultimate used invoke UI update and reset counter
        if(status == false)
        {
            OnUltimateStatusChanged.Invoke(0f);
            killsSinceLastUltimate = 0;
        }
    }

    private IEnumerator SpawnWaveOfEnemies()
    {
        while (true)
        {
            if (listOfAllEnemiesAlive.Count < maxEnemies)
            {
                Enemy clone = SpawnEnemy();
                yield return new WaitForEndOfFrame();
            }
            // Wait for the current randomized spawn delay
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
        
    }

    private int GetEnemiesRequiredForLevelUp()
    {
        return Mathf.FloorToInt(15 * Mathf.Pow(levelMultiplier, gameLevel - 1));
    }

    private void LevelUp()
    {
        gameLevel++;

        OnLevelUpChanged.Invoke(gameLevel);
        OnLevelPercentStatusChanged.Invoke(0f);

        maxEnemies += 2;
        Debug.Log($"Level Up! Current Level: {gameLevel}");

        // Every 10 levels, decrease the spawn delay range
        if (gameLevel % 2 == 0)
        {
            minSpawnDelay *= spawnDelayMultiplier;
            maxSpawnDelay *= spawnDelayMultiplier;

            // Clamp values to avoid zero or negative delays
            minSpawnDelay = Mathf.Max(minSpawnDelay, 0.1f);
            maxSpawnDelay = Mathf.Max(maxSpawnDelay, minSpawnDelay + 0.1f);

            Debug.Log($"New spawn delay: {minSpawnDelay} to {maxSpawnDelay}");
        }
    }
}