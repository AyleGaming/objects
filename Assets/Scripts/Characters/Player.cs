using UnityEngine;

public class Player : Character
{
    [SerializeField] private Transform playerWeaponTip;
    [SerializeField] private GameObject bulletReference;

    public override void Attack()
    {
        base.Attack();
        currentWeapon.Shoot(playerWeaponTip);
    }
}
