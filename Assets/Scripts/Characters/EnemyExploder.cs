using UnityEngine;

public class EnemyExploder : Enemy
{
    [SerializeField] private bool hasCollidedWithPlayer = false;
    private float speedMultiplier = 1.0f; // Initial speed multiplier
    private float accelerationRate = 0.1f; // Rate at which speed increases over time

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        healthValue.SetHealthValue(50);
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

    public override void Move(Vector2 direction)
    {
        // Increase the speed multiplier over time
        speedMultiplier += accelerationRate * Time.deltaTime;

        // Apply the scaled movement
        myRigidBody.AddForce(movementSpeed * speedMultiplier * Time.deltaTime * direction, ForceMode2D.Impulse);
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
