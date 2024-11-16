using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform[] spawnPointsArray;
    [SerializeField] private List<Enemy> listOfAllEnemiesAlive;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        StartCoroutine(SpawnWaveOfEnemies());
        SpawnEnemy();
    }

    void Update()
    {

    }

    private Enemy SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPointsArray.Length);
        Transform randomSpawnPoint = spawnPointsArray[randomIndex];

        Enemy enemyClone = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        listOfAllEnemiesAlive.Add(enemyClone);
        return enemyClone;
    }

    public void RemoveEnemyFromList(Enemy enemyToBeRemoved)
    {
        listOfAllEnemiesAlive.Remove(enemyToBeRemoved);
    }

    private IEnumerator SpawnWaveOfEnemies()
    {
        while (true)
        {
            if (listOfAllEnemiesAlive.Count < 20)
            {
                Enemy clone = SpawnEnemy();
                yield return new WaitForEndOfFrame();
//                clone.healthValue.OnDeath.AddListener(RemoveEnemyFromList);
            }
            yield return new WaitForSeconds(Random.Range(1, 4));
        }
        
    }

}