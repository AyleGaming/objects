using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class Meteor : Character
{
    [SerializeField] private Sprite[] meteorSprites; // Array of possible meteor sprites
    [SerializeField] private float minScale = 0.5f;  // Minimum size
    [SerializeField] private float maxScale = 2.0f;  // Maximum size
   
    private Transform player; // Reference to the player

    private void Awake()
    {
        // Assign a random sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = meteorSprites[Random.Range(0, meteorSprites.Length)];

        //Debug.Log(spriteRenderer.sprite);

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

        Destroy(gameObject, 10f);
    }

    protected override void Start()
    {
        base.Start();
        healthValue.SetHealthValue(150);
    }

}
