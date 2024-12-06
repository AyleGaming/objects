using UnityEngine;

public class EnemyExploder : Enemy
{
    private float speedMultiplier = 1.0f; // Initial speed multiplier
    private float accelerationRate = 0.1f; // Rate at which speed increases over time
    private int explodeDamage = 15;
    [SerializeField] private GameObject moveEffect;
    private GameObject particleEffectInstance;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        healthValue.SetHealthValue(50);

        particleEffectInstance = Instantiate(moveEffect, transform.position, Quaternion.identity);
        particleEffectInstance.transform.SetParent(transform); // Attach it to the enemy's transform
    }


    protected override void MoveEnemy(Vector2 direction)
    {
        // Increase the speed multiplier over time
        speedMultiplier += accelerationRate * Time.deltaTime;

        // Apply the scaled movement
        myRigidBody.AddForce(movementSpeed * speedMultiplier * Time.deltaTime * direction, ForceMode2D.Impulse);

        particleEffectInstance.transform.position = transform.position;
    }

    public override void Attack()
    {
        PlayDeathEffect();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            // Damage the player
            Attack();
            player.healthValue.DecreaseHealth(explodeDamage);
        }

        // Check if the collided object has a Meteor component
        Meteor meteor = collision.gameObject.GetComponent<Meteor>();
        if (meteor != null)
        {
            // Damage the meteor
            explodeDamage = 600;
            Attack();
            meteor.healthValue.DecreaseHealth(explodeDamage);
        }

    }
}
