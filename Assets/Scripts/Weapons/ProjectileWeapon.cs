using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    private GameObject projectilePrefab;

    public ProjectileWeapon(Transform tip, GameObject bulletReference) : base(tip)
    {
        projectilePrefab = bulletReference;
    }

    public override void Shoot()
    {
        // spawn a projectile
        GameObject.Instantiate(projectilePrefab, weaponTip.position, weaponTip.rotation);
    }

}
