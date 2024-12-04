using UnityEngine;

public class EnemySpitter : Enemy
{
    [SerializeField] private Transform weaponTip;

    protected override void Start()
    {
        base.Start();
        
    }

    public override void Attack()
    {
        if (attackTimer >= currentWeapon.fireRate)
        {
            currentWeapon.Shoot(weaponTip);
            attackTimer = 0;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }
}