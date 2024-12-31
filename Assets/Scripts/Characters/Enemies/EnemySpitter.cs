using UnityEngine;

public class EnemySpitter : Enemy
{
    [SerializeField] private Transform weaponTip;

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        attackTimer += Time.deltaTime;
    }

    public override void Attack()
    {
        if (attackTimer >= currentWeapon.fireRate)
        {
            currentWeapon.Shoot(weaponTip);
            attackTimer = 0;
        }
    }
}