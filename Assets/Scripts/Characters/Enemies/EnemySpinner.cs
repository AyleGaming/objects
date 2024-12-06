using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpinner : Enemy
{
    private float speedMultiplier = 1.0f; // Initial speed multiplier
    private float accelerationRate = 0.1f; // Rate at which speed increases over time
    private int cutDamage = 5;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private AudioClip[] attackSounds; // Sounds for attack
    [SerializeField] public float volume = 1.0f; // Volume for the sound

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        healthValue.SetHealthValue(60);

    }

    protected override void Update()
    {
        base.Update();
        RotateUFO();
    }

    public override void Attack()
    {
        base.Attack();
        if (attackTimer >= currentWeapon.fireRate)
        {
            target.healthValue.DecreaseHealth(cutDamage);
            attackTimer = 0;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }

    protected override void MoveEnemy(Vector2 direction)
    {
        // Increase the speed multiplier over time
        speedMultiplier += accelerationRate * Time.deltaTime;

        // Apply the scaled movement
        myRigidBody.AddForce(movementSpeed * speedMultiplier * Time.deltaTime * direction, ForceMode2D.Impulse);
        // Spin me right round
        myRigidBody.AddTorque(rotationSpeed * Time.deltaTime, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            // Damage the player
            Attack();
            PlayRandomAttackSound(transform.position);
           
            //            player.healthValue.DecreaseHealth(cutDamage);
        }
    }

    private void PlayRandomAttackSound(Vector3 position)
    {
        if (attackSounds != null && attackSounds.Length > 0)
        {
            // Choose a random sound from the array
            int randomIndex = Random.Range(0, attackSounds.Length);
            AudioClip selectedSound = attackSounds[randomIndex];

            // Play sound
            PlaySoundAtPosition(selectedSound, position, 1.2f, 1.4f);
        }
    }

    private void RotateUFO()
    {
        // Rotate around the Z-axis for 2D rotation
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
