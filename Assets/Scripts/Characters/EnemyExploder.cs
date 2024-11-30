using UnityEngine;

public class EnemyExploder : Enemy
{
    [SerializeField] private bool hasCollidedWithPlayer = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        // Explode when collided with player
        if(hasCollidedWithPlayer == true)
        {
            Attack();
        }
    }

    public override void Attack()
    {
        // Decrease player max health by max (3)
        target.healthValue.DecreaseHealth(15);
        PlayDeathEffect();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollidedWithPlayer = true;
        }
    }
}
