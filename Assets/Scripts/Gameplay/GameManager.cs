using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
   
    public UnityEvent OnGameStart;
    public UnityEvent OnGameOver;

    public UnityEvent<float> OnUltimateStatusChanged;
    public UnityEvent<int> OnLevelUpChanged;
    public UnityEvent<float> OnLevelPercentStatusChanged;
    
    public static event Action<ScoreType> OnEnemyKilled;
    public static event Action<GameManager> OnGameManagerInit;

    [SerializeField] private PickUp[] pickUpPrefabs;
    [SerializeField] private GameObject pickUpEffect;
       
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform[] spawnPointsArray;
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private List<Enemy> listOfAllEnemiesAlive;

    [SerializeField] private Meteor[] meteorPrefabs; // Assign in Inspector
    [SerializeField] private Transform[] spawnPoints; // Assign spawn points in Inspector

    [SerializeField] private int totalEnemiesKilled = 0;
    [SerializeField] private int gameLevel = 1;

    private int maxEnemies = 10;
    private float minSpawnDelay = 1f; // Minimum delay for spawning
    private float maxSpawnDelay = 3f; // Maximum delay for spawning
    private float spawnDelayMultiplier = 0.8f; // Multiplier to reduce delay every 10 levels

    private float levelMultiplier = 1.2f; // increase amount of enemies required per level
    private int levelKillCount = 0; // number of enemies killed per level
    private int killsForUltimate = 5;
    private int killsSinceLastUltimate = 0;

    private void OnEnable()
    {
        Character.OnCharacterInitialized += SetupCharacterListeners;
    }

    private void OnDisable()
    {
        Character.OnCharacterInitialized -= SetupCharacterListeners;
    }

    private void SetupCharacterListeners(Character character)
    {
        if (character is Player player)
        {
            if (player.healthValue != null)
            {
                player.healthValue.OnDeath.AddListener(GameOver);
                player.OnUltimateStatusAvailable.AddListener(EnablePlayerUltimate);
            }
            else
            {
                Debug.LogError("GameManager: Player's healthValue is not initialized.");
            }
        }
    }

    void Start()
    {
        OnGameManagerInit?.Invoke(this);

        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        StartCoroutine(SpawnWaveOfEnemies());
    }

    private PickUp SpawnPickUp(Enemy enemyToBeRemoved)
    {
        PickUp randomPrefab = pickUpPrefabs[UnityEngine.Random.Range(0, pickUpPrefabs.Length)];
        PickUp pickUpClone = Instantiate(randomPrefab, enemyToBeRemoved.transform.position, Quaternion.identity);

        return pickUpClone;
    }

    public void EnemyKilled(ScoreType scoreType)
    {
        OnEnemyKilled?.Invoke(scoreType);
    }

    public void RemoveEnemyFromList(Enemy enemyToBeRemoved)
    {
        totalEnemiesKilled++;
        killsSinceLastUltimate++;
        levelKillCount++;


        Debug.Log("RemoveEnemyFromList: " + enemyToBeRemoved);

        listOfAllEnemiesAlive.Remove(enemyToBeRemoved);
        listOfAllEnemiesAlive.RemoveAll(enemy => enemy == null); // list wasn't being cleaned

        EnemyKilled(ScoreType.EnemyKilled);

        if (totalEnemiesKilled % 6 == 0)
        {
            SpawnPickUp(enemyToBeRemoved);
        }

        // Determine level up
        int requiredEnemies = GetEnemiesRequiredForLevelUp();

        if (levelKillCount >= requiredEnemies)
        {
            LevelUp();
            levelKillCount = 0;
        }

        if (totalEnemiesKilled % killsForUltimate == 0)
        {
            EnablePlayerUltimate(true);
        }

        if(killsSinceLastUltimate > killsForUltimate)
        {
            OnUltimateStatusChanged.Invoke(100f);
        }
        else
        {
            float killsToUltimatePercent = (float)killsSinceLastUltimate / killsForUltimate * 100;
            OnUltimateStatusChanged.Invoke(killsToUltimatePercent);
        }

        float killPercentToLevel = ((float)levelKillCount / (float)requiredEnemies) * 100;
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
                if (UnityEngine.Random.value > 0.5f) // 30% chance to spawn a meteor
                {
                    SpawnEnemies(); // Spawn meteor
                }
                Enemy enemy = SpawnEnemy(); // Spawn regular enemy
                listOfAllEnemiesAlive.Add(enemy);
            }
            // Wait for the current randomized spawn delay
            float delay = UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
        
    }

    private Enemy SpawnEnemy()
    {
        // Select a random spawn point
        int randomIndex = UnityEngine.Random.Range(0, spawnPointsArray.Length);
        Transform randomSpawnPoint = spawnPointsArray[randomIndex];

        // Determine the enemy type to spawn
        Enemy enemyToSpawn;

        if (gameLevel <= enemyPrefabs.Length)
        {
            // Spawn the same enemy for the current level (1-based indexing)
            enemyToSpawn = enemyPrefabs[gameLevel - 1];
        }
        else
        {
            // If beyond the number of enemy types, choose randomly from all available
            enemyToSpawn = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        }

        // Instantiate and track the spawned enemy
        Enemy enemyClone = Instantiate(enemyToSpawn, randomSpawnPoint.position, randomSpawnPoint.rotation);
        listOfAllEnemiesAlive.Add(enemyClone);

        return enemyClone;
    }

    private void SpawnEnemies()
    {
        // Spawn a meteor at a random spawn point
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Meteor meteorToSpawn = meteorPrefabs[UnityEngine.Random.Range(0, meteorPrefabs.Length)];
        Instantiate(meteorToSpawn, spawnPoint.position, Quaternion.identity);
        
    }


    private int GetEnemiesRequiredForLevelUp()
    {
        return Mathf.FloorToInt(3 * Mathf.Pow(levelMultiplier, gameLevel - 1));
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

    private void GameOver()
    {
        OnGameOver.Invoke();
        StopAllCoroutines();
    }

    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}