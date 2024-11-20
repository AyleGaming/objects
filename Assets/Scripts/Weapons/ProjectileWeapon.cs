using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Projectile Weapon")]

public class ProjectileWeapon : Weapon
{
    [SerializeField] private Bullet projectilePrefab;

    public override void Shoot(Transform weaponTip)
    {
        // spawn a projectile
        Bullet bulletClone = GameObject.Instantiate(projectilePrefab, weaponTip.position, weaponTip.rotation);
        bulletClone.InitializeBullet(damage);
    }

    public override void Reload()
    {

    }

}
