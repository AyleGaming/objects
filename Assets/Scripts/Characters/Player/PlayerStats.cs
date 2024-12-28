using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


public enum AbilitiesModifiable
{
    health,
    attackDamage,
    damageReduction,
    movementSpeed,
    gunsBase,
    additionalGuns,
    attackSpeed,
    hasUltimateAbility,
    hasBlinkAbility,
    hasShieldAbility,
    additiveWeaponDamage
}

public enum AbilitiesModifiableSecondary
{
    none,
    ultimateCooldown,
    blinkCooldown,
    shield,
    additionalGunsTime,
}

public enum AbilityDataType
{
   stats,
   stats_int,
   stats_bool
}


public class PlayerStats : MonoBehaviour
{
    private Dictionary<string, float> stats = new Dictionary<string, float>()
    {
        { "damageReduction", 0f },
        { "movementSpeed", 18f },
        { "ultimateCooldown", 30f },
        { "blinkCooldown", 10f },
        { "attackSpeed", .30f },
        { "damageMultiplier", 1f },
        { "additionalGunsTime", 10f },
        { "additiveWeaponDamage", 0f }
    };

    private Dictionary<string, int> statsInt = new Dictionary<string, int>()
    {
        { "health", 75 },
        { "shield", 25 },
        { "attackDamage", 25 },
        { "gunsBase", 1 },
        { "gunsActive", 1 },
    };

    private Dictionary<string, bool> statsBool = new Dictionary<string, bool>()
    {
        { "hasUltimateAbility", false },
        { "hasBlinkAbility", true },
        { "hasShieldAbility", false }
    };

    public void SetStat(string statName, float newValue)
    {
        if (stats.ContainsKey(statName))
        {
            stats[statName] = newValue;
            Debug.Log($"FLOAT: {statName}: {newValue}");
        }
        else
        {
            Debug.LogError($"Stat {statName} not found in stats dictionary.");
        }
    }

    public void SetStatInt(string statName, int newValue)
    {
        if (statsInt.ContainsKey(statName))
        {
            statsInt[statName] = newValue;
            Debug.Log($"INT: {statName}: {newValue}");
        }
        else
        {
            Debug.LogError($"Stat {statName} not found in stats dictionary.");
        }
    }

    public void SetStatBool(string statName, bool newValue)
    {
        if (statsBool.ContainsKey(statName))
        {
            statsBool[statName] = newValue;
            Debug.Log($"BOOL: {statName}: {newValue}");
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

    public int GetStatInt(string statName)
    {
        return statsInt[statName];
    }

    public bool GetStatBool(string statName)
    {
        return statsBool[statName];
    }

}

