using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Core Stats")]
    public int health = 75;
    public int shield = 25;
    public int damage = 25;
    public int gunsBase = 1;
    public int additionalGuns = 1;
    public float additionalGunsTime = 10f;
    public float speed = 15f;
    public float attackSpeed = 1.8f;
    public float blinkCooldown = 5f;

    [Header("Abilities")]
    public bool hasUltimateAbility = false;
    public bool hasBlinkAbility = false;
    public bool hasShieldAbility = false;

    public void IncreaseHealth(int amount)
    {
        health += amount;
        Debug.Log($"Health increased to: {health}");
    }

    public void IncreaseDamage(int amount)
    {
        damage += amount;
        Debug.Log($"Damage increased to: {damage}");
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log($"Speed increased to: {speed}");
    }
}
