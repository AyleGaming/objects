using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


public enum AbilitiesModifiable
{
    health,
    damage,
    movementSpeed,
    gunsBase,
    additionalGuns,
    attackSpeed,
    hasUltimateAbility,
    hasBlinkAbility,
    hasShieldAbility,
}

public enum AbilitiesModifiableSecondary
{
    none,
    ultimateCooldown,
    blinkCooldown,
    shield,
    additionalGunsTime,
}


public class PlayerStats : MonoBehaviour
{
    [Header("Core Stats")]
    public int health = 75;
    public int damage = 25;
    public float movementSpeed = 15f;
   
    [Header("Weapons")]
    public int gunsBase = 1;
    public int gunsActive = 1;
    public int additionalGuns = 1;
    public float additionalGunsTime = 10f;
    public float attackSpeed = 1.8f;

    [Header("Abilities")]
    public bool hasUltimateAbility = false;
    public float ultimateCooldown = 30f;
    
    public bool hasBlinkAbility = false;
    public float blinkCooldown = 5f;
    
    public bool hasShieldAbility = false;
    public int shield = 25;


    private Dictionary<string, float> stats = new Dictionary<string, float>()
    {
        { "health", 75f },
        { "shield", 25f },
        { "damage", 25f },
        { "movementSpeed", 15f },
        { "gunsBase", 1f },
        { "gunsActive", 1f },
        { "ultimateCooldown", 30f },
        { "blinkCooldown", 10f },
        { "attackSpeed", .18f },
        { "damageMultiplier", 1f }
    };

    public void SetStat(string statName, float newValue)
    {
        if (stats.ContainsKey(statName))
        {
            stats[statName] = newValue;
            Debug.Log($"{statName}: {newValue}");
            // Call a method on the Player to apply the updated stat
            //            ApplyStatToPlayer(statName);
        }
        else
        {
            Debug.LogError($"Stat {statName} not found in stats dictionary.");
        }
    }

    public float GetStat(string statName)
    {
        return stats[statName];
    }

}

