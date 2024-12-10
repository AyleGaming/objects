using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnityEvent OnGameStart;
    public UnityEvent OnGameOver;

    public UnityEvent<float> OnUltimateStatusChanged;
    public UnityEvent<int> OnLevelUpChanged;
    public UnityEvent<float> OnLevelPercentStatusChanged;
    
    public static event Action<ScoreType> OnEnemyKilled;
    public static event Action<GameManager> OnGameManagerInit;

    [SerializeField] private PickUp[] pickUpPrefabs;
    [SerializeField] private GameObject pickUpEffect;
       
    [SerializeField] private Transform[] spawnPointsArray;
    [SerializeField] private List<Enemy> listOfAllEnemiesAlive;

    [SerializeField] private Meteor[] meteorPrefabs; // Assign in Inspector
    [SerializeField] private Transform[] spawnPoints; // Assign spawn points in Inspector

    [SerializeField] private int totalEnemiesKilled = 0;
    [SerializeField] private int gameLevel = 1;

    [SerializeField] private bool enableEnemiesSpawn = true;
    [SerializeField] private bool enableMeteorsSpawn = true;
    [SerializeField] private int baseEnemiesToLevelUp = 10;

    [SerializeField] private EnemyType selectedEnemyType = EnemyType.Earth;
    [SerializeField] private EnemySuperType selectedEnemySuperType = EnemySuperType.Base;

    private int maxEnemies = 10;
    private float minSpawnDelay = 2f; // Minimum delay for spawning
    private float maxSpawnDelay = 4f; // Maximum delay for spawning
    private float spawnDelayMultiplier = 0.95f; // Multiplier to reduce delay every 10 levels

    private float levelMultiplier = 1.1f; // increase amount of enemies required per level
    private int levelKillCount = 0; // number of enemies killed per level
    [SerializeField] float killsForUltimate = 20f;
    private float killsSinceLastUltimate = 0f;
    private float meteorSpawnChance = 0.85f;

    [SerializeField] private PowerLibrary powerLibrary;
    [SerializeField] private PowerUpUIManager PowerUpUIManager;
    public EnemyManager enemyManager;

    private void Awake()
    {
        // Ensure there's only one instance of GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate GameManagers
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    void Start()
    {
        OnGameManagerInit?.Invoke(this);
        StartCoroutine(SpawnWaveOfEnemies());
    }

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
            Debug.Log($"totalEnemiesKilled: {totalEnemiesKilled}");
            EnablePlayerUltimate(true);
        }

        if(killsSinceLastUltimate >= killsForUltimate)
        {
            OnUltimateStatusChanged.Invoke(100f);
        }
        else
        {
            float killsToUltimatePercent = killsSinceLastUltimate / killsForUltimate * 100;
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
                // Meteor spawning
                if (UnityEngine.Random.value > meteorSpawnChance) // 30% chance to spawn a meteor
                {
                    if (enableMeteorsSpawn)
                    {
                        SpawnMeteors(); // Spawn meteor
                    }
                }
                if (enableEnemiesSpawn)
                {
                    Enemy enemy = SpawnNewEnemy(); // Spawn regular enemy
                    listOfAllEnemiesAlive.Add(enemy);
                }
                    
            }
            // Wait for the current randomized spawn delay
            float delay = UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
        
    }

    private void DetermineEnemyType()
    {
        selectedEnemyType = (EnemyType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);
        Debug.Log($"NEW ENEMYTYPE: {selectedEnemyType}");
    }

    public void DetermineEnemySuperType()
    {
        // Change to the next SuperType in the sequence
        selectedEnemySuperType = (EnemySuperType)(((int)selectedEnemySuperType + 1) % Enum.GetValues(typeof(EnemySuperType)).Length);
        Debug.Log($"NEW SUPERTYPE: {selectedEnemySuperType}");
    }

    private Enemy SpawnNewEnemy()
    {
        // Select a random spawn point
        int randomIndex = UnityEngine.Random.Range(0, spawnPointsArray.Length);
        Transform randomSpawnPoint = spawnPointsArray[randomIndex];
        Enemy enemyClone = enemyManager.SpawnEnemy(randomSpawnPoint, selectedEnemyType, selectedEnemySuperType);
              
        return enemyClone;
    }

    private void SpawnMeteors()
    {
        // Spawn a meteor at a random spawn point
        int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Meteor meteorToSpawn = meteorPrefabs[UnityEngine.Random.Range(0, meteorPrefabs.Length)];
        Instantiate(meteorToSpawn, spawnPoint.position, Quaternion.identity);
    }

    private int GetEnemiesRequiredForLevelUp()
    {
        return Mathf.FloorToInt(baseEnemiesToLevelUp * Mathf.Pow(levelMultiplier, gameLevel - 1));
    }

    public void UnlockNewPowers()
    {
        PowerData selectedAttack = powerLibrary.GetRandomPower(PowerCategory.Attack, gameLevel, RollPowerRarity());
        PowerData selectedDefense = powerLibrary.GetRandomPower(PowerCategory.Defense, gameLevel, RollPowerRarity());
        PowerData selectedSpecial = powerLibrary.GetRandomPower(PowerCategory.Special, gameLevel, RollPowerRarity());

        Debug.Log($"selectedAttack: {selectedAttack}, selectedDefense: {selectedDefense}, selectedSpecial:{selectedSpecial}");

        // Populate and show LevelUp UI
        PowerUpUIManager.PopulatePowerUpUI(new List<PowerData> { selectedAttack, selectedDefense, selectedSpecial });
        PowerUpUIManager.Show();
    }

    // Determine what powers to level
    private PowerRarity RollPowerRarity()
    {
        int randomRoll = UnityEngine.Random.Range(1, 100);
        PowerRarity powerRarity = PowerRarity.common;

        Debug.Log($"randomRoll: {randomRoll}");

        if (randomRoll > 80 && randomRoll < 95)
        {
            powerRarity = PowerRarity.rare;
        }
        else if (randomRoll >= 95)
        {
            powerRarity = PowerRarity.legendary;
        }

        return powerRarity;
    }

    private void LevelUp()
    {
        gameLevel++;

        OnLevelUpChanged.Invoke(gameLevel);
        OnLevelPercentStatusChanged.Invoke(0f);

        maxEnemies += 2;
//        Debug.Log($"Level Up! Current Level: {gameLevel}");

        // Every 10 levels, decrease the spawn delay range
        if (gameLevel % 2 == 0)
        {
            minSpawnDelay *= spawnDelayMultiplier;
            maxSpawnDelay *= spawnDelayMultiplier;
            meteorSpawnChance *= spawnDelayMultiplier;

            // Clamp values to avoid zero or negative delays
            minSpawnDelay = Mathf.Max(minSpawnDelay, 0.1f);
            maxSpawnDelay = Mathf.Max(maxSpawnDelay, minSpawnDelay + 0.1f);
            meteorSpawnChance = Mathf.Max(meteorSpawnChance, 0.1f);
            DetermineEnemyType();

            //            Debug.Log($"New spawn delay: {minSpawnDelay} to {maxSpawnDelay}: METEORS: {meteorSpawnChance}: killsForUltimate: {killsForUltimate}");


            UnlockNewPowers();
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