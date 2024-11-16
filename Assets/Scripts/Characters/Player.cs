using UnityEngine;

public class Player : Character
{
    [SerializeField] private Transform playerWeaponTip;
    [SerializeField] private GameObject bulletReference;

    protected override void Start()
    {
        base.Start();
        currentWeapon = new ProjectileWeapon(playerWeaponTip, bulletReference);
    }
}
