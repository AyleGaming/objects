using UnityEngine;
using UnityEngine.Events;

public class Health
{
    public UnityEvent OnDeath;
    private float healthValue;

    public void DecreaseHealth(float damage)
    {
        healthValue -= damage;
       
        if (IsDead())
        {
            OnDeath.Invoke();
        }
    }

    public void IncreaseHealth(float damage)
    {
        healthValue += damage;
        
    }

    public bool IsDead()
    {
        return healthValue <= 0;
    }

    public Health()
    {
        healthValue = 3;
        OnDeath = new UnityEvent();
        
    }


    public Health(float initialHealth)
    {
        healthValue = initialHealth;
        OnDeath = new UnityEvent();
    }

}