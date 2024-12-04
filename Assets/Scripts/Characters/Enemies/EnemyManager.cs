using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPrefabGroup
    {
        public List<Enemy> baseEnemies;
        public List<Enemy> insectoidEnemies;
        public List<Enemy> abyssalEnemies;
    }

    [SerializeField] private EnemyPrefabGroup[] enemyPrefabsBySuperType; // Enemy prefabs grouped by SuperType

    private EnemySuperType currentSuperType = EnemySuperType.Base;
    private EnemyType currentEnemyType = EnemyType.Earth;

    // This method will spawn an enemy based on the current super type and enemy type
    public Enemy SpawnEnemy(Transform spawnPointTransform, EnemyType selectedEnemyType, EnemySuperType selectedEnemySuperType)
    {
        List<Enemy> selectedPrefabList = null;

        currentEnemyType = selectedEnemyType;
        currentSuperType = selectedEnemySuperType;

        // Determine which prefab list to choose from based on the current super type
        switch (selectedEnemySuperType)
        {
            case EnemySuperType.Base:
                selectedPrefabList = enemyPrefabsBySuperType[0].baseEnemies;
                break;
            case EnemySuperType.Insectoid:
                selectedPrefabList = enemyPrefabsBySuperType[1].insectoidEnemies;
                break;
            case EnemySuperType.Abyssal:
                selectedPrefabList = enemyPrefabsBySuperType[2].abyssalEnemies;
                break;
        }
        
        int randomPrefab = UnityEngine.Random.Range(0, selectedPrefabList.Count); // Assuming 2 variants (Spitter, Biter)
        int variantID = randomPrefab + 1;

        // Now select a random enemy from the list for the chosen enemy type
        Enemy selectedEnemyPrefab = selectedPrefabList[randomPrefab];
        Enemy newEnemy = Instantiate(selectedEnemyPrefab, spawnPointTransform.transform.position, Quaternion.identity);


        // Here you can add further logic to change the abilities of the enemy based on EnemyType
        //SetupEnemyAttributes(enemy);

        // Pass the EnemyType and VariantID to the enemy
        newEnemy.SetupEnemy(selectedEnemyType, variantID);

        return newEnemy;
    }

    // Sets up enemy attributes based on the selected enemy type
    private void SetupEnemyAttributes(Enemy enemy)
    {
        switch (currentEnemyType)
        {
            case EnemyType.Earth:
                //enemy.SetEarthAbilities(); // Define this method on Enemy to adjust abilities
                break;
            case EnemyType.Fire:
                //enemy.SetFireAbilities();
                break;
            case EnemyType.Ice:
                //enemy.SetIceAbilities();
                break;
            case EnemyType.Poison:
                //enemy.SetPoisonAbilities();
                break;
        }
    }
   
}
