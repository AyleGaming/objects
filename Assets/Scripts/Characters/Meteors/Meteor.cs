using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class Meteor : Character
{
    [SerializeField] private float minScale = 0.5f;  // Minimum size
    [SerializeField] private float maxScale = 2.0f;  // Maximum size
    [SerializeField] private GameObject collisionEffect;
    [SerializeField] private GameObject pickUpShieldPrefab;
    [SerializeField] private GameObject pickUpGunsPrefab;
    [SerializeField] private GameObject pickUpHealthPrefab;
    [SerializeField] private Sprite[] meteorSprites; // Array of meteor sprites

    private GameObject prefabToSpawnOnDeath;
    private int meteorCollisionDamage = 2;
    protected Player target;
    protected float attackTimer;
    private Transform player; // Reference to the player
    private int startingHealthValue = 50;

    public enum MeteorSize
    {
        Small,
        Medium,
        Large
    }

    private void Awake()
    {
        // Assign a random sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = meteorSprites[Random.Range(0, meteorSprites.Length)];
        
        if (spriteRenderer.sprite.name.Contains("Gems"))
        {
            DetermineMeteorPickUpToSpawn(spriteRenderer);
        }

        MeteorSize meteorSize = GetMeteorSizeFromSprite(spriteRenderer.sprite.name);
        startingHealthValue = GetHealthBasedOnSize(meteorSize);

        // Randomize scale for size variation
        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, 1);

        // Find the player
        player = FindObjectOfType<Player>().transform;

        // Apply initial velocity toward the player
        if (player != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized; // Get direction to player
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = directionToPlayer * movementSpeed; // Set velocity toward the player
        }

        // Randomize spin
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.angularVelocity = Random.Range(-200f, 200f);

        Destroy(gameObject, 20f);
    }


    protected override void Start()
    {
        base.Start();
        healthValue.SetHealthValue(startingHealthValue);
    }

    public override void Attack()
    {
        base.Attack();
        // Decrease both player and meteor health
        if (target != null) {
            target.healthValue.DecreaseHealth(meteorCollisionDamage);
            healthValue.DecreaseHealth(meteorCollisionDamage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(collisionEffect, collision.transform.position, Quaternion.identity);
            if(collisionSound != null)
            {
                PlaySoundAtPosition(collisionSound, collision.transform.position, 1.2f, 1.6f); 
            }
            target = collision.gameObject.GetComponent<Player>();
            Attack();
        }
    }

    public override void PlayDeathEffect()
    {
        base.PlayDeathEffect();
        GameManager.Instance.EnemyKilled(ScoreType.AsteroidDestroyed);
        if (prefabToSpawnOnDeath != null)
        {
            Instantiate(prefabToSpawnOnDeath, transform.position, Quaternion.identity);
        }
        PlaySoundAtPosition(collisionSound, transform.position, 0.5f, 0.8f);
    }

    private void DetermineMeteorPickUpToSpawn(SpriteRenderer spriteRenderer)
    {
        string spriteName = spriteRenderer.sprite.name;
        GameObject pickupPrefab = null;

        switch (spriteName)
        {
            case string name when name.Contains("Gems_A"):
                pickupPrefab = pickUpShieldPrefab;
                break;
            case string name when name.Contains("Gems_B"):
                pickupPrefab = pickUpGunsPrefab;
                break;
            case string name when name.Contains("Gems_C"):
                pickupPrefab = pickUpHealthPrefab;
                break;
            case string name when name.Contains("Gems_D"):
                pickupPrefab = pickUpGunsPrefab;
                break;
            default:
                Debug.LogWarning($"No matching prefab for sprite: {spriteName}");
                break;
        }

        if (pickupPrefab != null)
        {
            prefabToSpawnOnDeath = pickupPrefab;
        }
    }

    private MeteorSize GetMeteorSizeFromSprite(string spriteName)
    {
        // You can map based on the sprite name pattern or use a switch/if-else structure
        switch (spriteName)
        {
            case string name when name.Contains("Meteor_1") || name.Contains("Meteor_2"):
                return MeteorSize.Small;
            case string name when name.Contains("Meteor_3") || name.Contains("Meteor_4"):
                return MeteorSize.Medium;
            case string name when name.Contains("Meteor_5"):
                return MeteorSize.Large;
            default:
                Debug.LogWarning("Unknown meteor size: " + spriteName);
                return MeteorSize.Small;  // Default to small if name doesn't match
        }
    }

    private int GetHealthBasedOnSize(MeteorSize meteorSize)
    {
        int healthValue;

        switch (meteorSize)
        {
            case MeteorSize.Small:
                healthValue = startingHealthValue;  // Small meteors have lower health
                break;
            case MeteorSize.Medium:
                healthValue = 75; // Medium meteors have medium health
                break;
            case MeteorSize.Large:
                healthValue = 125; // Large meteors have higher health
                break;
            default:
                healthValue = startingHealthValue; // Default small if size isn't recognized
                break;
        }

        return healthValue;
    }
}
