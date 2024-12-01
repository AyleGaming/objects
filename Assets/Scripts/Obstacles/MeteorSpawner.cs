using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [SerializeField] private Sprite[] meteorSprites; // Array of meteor sprites
    [SerializeField] private float minScale = 0.5f;  // Minimum size
    [SerializeField] private float maxScale = 2.0f;  // Maximum size


    private void Update()
    {
        
    }

    public GameObject SpawnMeteor(Vector2 spawnPosition)
    {
        // Create a new GameObject for the meteor
        GameObject meteor = new GameObject("Meteor");

        // Add a SpriteRenderer and assign a random sprite
        SpriteRenderer spriteRenderer = meteor.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = meteorSprites[Random.Range(0, meteorSprites.Length)];

        // Add a Rigidbody2D for physics
        Rigidbody2D rb = meteor.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Optional: Set gravity if meteors shouldn't fall naturally
        rb.mass = Random.Range(1f, 5f); // Randomize mass for variation
        rb.angularVelocity = Random.Range(-200f, 200f);

        // Add a PolygonCollider2D for collision
        PolygonCollider2D collider = meteor.AddComponent<PolygonCollider2D>();

        // Randomize scale for size variation
        float randomScale = Random.Range(minScale, maxScale);
        meteor.transform.localScale = new Vector3(randomScale, randomScale, 1);

        // Set the position
        meteor.transform.position = spawnPosition;

        return meteor;
    }
}
