using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float distanceToStop = 3f;
    [SerializeField] private float attackCooldown = 3f;

    protected Player target;
    protected float attackTimer;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<Player>();
    }

    protected override void Update()
    {
        // dead?
        if (target == null) return;

        Vector2 destination = target.transform.position;
        Vector2 currentPosition = transform.position;
        Vector2 direction = destination - currentPosition;

        if(Vector2.Distance(destination, currentPosition) > distanceToStop)
        {
            Move(direction.normalized);
        } else
        {
            Attack();
        }
        Look(direction.normalized);
    }

    public override void Attack()
    {
        base.Attack();
        if(attackTimer >= attackCooldown)
        {
            target.healthValue.DecreaseHealth(1);
            attackTimer = 0;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }

    public override void PlayDeathEffect()
    {
        GameManager.instance.RemoveEnemyFromList(this);
        base.PlayDeathEffect();
    }
}
