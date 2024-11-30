using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMultishot : Enemy
{
    [SerializeField] private Transform weaponTipLeft;
    [SerializeField] private Transform weaponTipRight;
    public override void Attack()
    {
        if (attackTimer >= currentWeapon.fireRate)
        {
            currentWeapon.Shoot(weaponTipLeft);
            currentWeapon.Shoot(weaponTipRight);
            attackTimer = 0;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }
}
