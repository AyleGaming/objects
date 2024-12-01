using UnityEngine;
using UnityEngine.Events;

public class Health
{
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnShieldChanged;
    public UnityEvent OnDeath;

    private float healthValue;
    private float shieldValue;

    public void DecreaseHealth(float damage)
    {
        if(shieldValue > 0)
        {
            // 40 damage = 40-15
            // total damage = 15
            // shield = 0
            if(damage >= shieldValue)
            {
                damage -= shieldValue;
                shieldValue = 0;
            } else
            {
                shieldValue -= damage;
                damage = 0;
            }
            OnShieldChanged.Invoke(shieldValue);
        }

        healthValue -= damage;
        OnHealthChanged.Invoke(healthValue);

        // Update UI check if is dead
        if (IsDead())
        {
            OnDeath.Invoke();
        }
    }

    public void IncreaseHealth(float increaseParamater)
    {
        healthValue += increaseParamater;
        OnHealthChanged.Invoke(healthValue);
    }
    public void IncreaseShield(float increaseParamater)
    {
        shieldValue += increaseParamater;
        OnShieldChanged.Invoke(shieldValue);
    }

    public float GetHealthValue()
    {
        return healthValue;
    }

    public float GetShieldValue()
    {
        return shieldValue;
    }

    public void SetHealthValue(float value)
    {
        healthValue = value;
    }

    public void SetShieldValue(float value)
    {
        shieldValue = value;
    }
    public bool IsDead()
    {
        return healthValue <= 0;
    }

    public bool HasShield()
    {
        return shieldValue > 0;
    }

    public Health()
    {
        healthValue = 75;
        shieldValue = 25;
        OnDeath = new UnityEvent();
        OnHealthChanged = new UnityEvent<float>();
        OnShieldChanged = new UnityEvent<float>();
    }

    public Health(float initialHealth, float initialShield)
    {
        healthValue = initialHealth;
        shieldValue = initialShield;
        OnDeath = new UnityEvent();
        OnHealthChanged = new UnityEvent<float>();
        OnShieldChanged = new UnityEvent<float>();
    }
}