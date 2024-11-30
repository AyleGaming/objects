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
        PlayRandomFireSound(weaponTip.position);
        Bullet bulletClone = GameObject.Instantiate(projectilePrefab, weaponTip.position, weaponTip.rotation);
        bulletClone.InitializeBullet(damage);
    }

    private void PlayRandomFireSound(Vector3 position)
    {
        if (fireSounds != null && fireSounds.Length > 0)
        {
            // Choose a random sound from the array
            int randomIndex = Random.Range(0, fireSounds.Length);
            AudioClip selectedSound = fireSounds[randomIndex];

            // Play the selected sound
            AudioSource.PlayClipAtPoint(selectedSound, position, volume);
        }
    }


    public override void Reload()
    {

    }
}
