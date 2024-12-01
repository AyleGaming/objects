using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Faction bulletFaction;
    [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject collisionEffect;
    private float myDamage;
  
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody.velocity = transform.up * bulletSpeed;
        Destroy(gameObject, 5f);
    }

    public void InitializeBullet(Faction faction, float damage)
    {
        bulletFaction = faction;
        myDamage = damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the Character component from the collided object
        Character character = collision.rigidbody.GetComponent<Character>();

        if (character != null)
        {
            // Only apply damage if the factions are opposing
            if ((bulletFaction == Faction.Player && character.CompareTag("Enemy")) ||
                (bulletFaction == Faction.Enemy && character.CompareTag("Player")))
            {
                character.healthValue.DecreaseHealth(myDamage);
                Instantiate(collisionEffect, transform.position, Quaternion.identity);
            }
        }

        // Destroy the bullet regardless of collision target
        Destroy(gameObject);
    }

}
