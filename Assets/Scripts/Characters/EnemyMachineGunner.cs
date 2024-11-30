using UnityEngine;

public class EnemyMachineGunner : Enemy
{
    [SerializeField] private Transform weaponTip;
    public override void Attack()
    {
        if(attackTimer >= currentWeapon.fireRate)
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