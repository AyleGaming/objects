using UnityEngine;
using UnityEngine.Events;
using System;

public class Character : MonoBehaviour
{
    public UnityEvent<float> BlinkCoolDownUpdate;
    public static event Action<Character> OnCharacterInitialized;

    [SerializeField] protected Rigidbody2D myRigidBody;
    [SerializeField] protected float movementSpeed = 10f;
    [SerializeField] private GameObject dieEffect;
    [SerializeField] public bool ultimateAvailable = false;
    [SerializeField] protected AudioClip collisionSound;
    public Health healthValue;
    public Weapon currentWeapon;

    protected bool checkForCollisions = false;
    protected int blinkDistance = 1;
    protected float blinkCooldown = 5f;
    protected float nextBlinkTime = 0f;
    private bool isBlinking = false;
    public bool blinkAvailable = true;

    public LayerMask obstacleMask;
   

    protected virtual void Start()
    {
        healthValue = new Health();
        healthValue.OnDeath.AddListener(PlayDeathEffect);
        OnCharacterInitialized?.Invoke(this);
        ultimateAvailable = false;
    }

    protected virtual void Update()
    {
        
    }

    public virtual void Move(Vector2 direction)
    {
       if (isBlinking) return;
       myRigidBody.AddForce(movementSpeed * Time.deltaTime * direction, ForceMode2D.Impulse);
    }

    public virtual void Look(Vector2 direction)
    {
        if (isBlinking) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //angle = Vector2.Angle(Vector2.right, direction);

        myRigidBody.SetRotation(angle - 90f);
    }

    public virtual void PlayDeathEffect()
    {
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public virtual void Attack()
    {
        
    }

    public virtual void UltimateAttack()
    {

    }
    public virtual void Blink(Vector3 lookDirection)
    {
        if (blinkAvailable)
        {
            isBlinking = true; // Disable movement during blink

            // Calculate the target position
            Vector2 blinkDirection = lookDirection;
            Vector3 targetPosition = (Vector2)transform.position + blinkDirection * blinkDistance;

            // Optional: Check for collisions (e.g., walls, obstacles)
            if (checkForCollisions)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, blinkDirection, blinkDistance, obstacleMask);
                if (hit.collider != null)
                {
                    // Stop at the obstacle
                    targetPosition = hit.point;
                }
            }

            // Teleport the player
            myRigidBody.MovePosition(targetPosition);

            // Re-enable movement after blink
            Invoke(nameof(EndBlink), 0.1f);
        }
     
    }
    private void EndBlink()
    {
        isBlinking = false; // Re-enable movement
        nextBlinkTime = Time.time + blinkCooldown;
        BlinkCoolDownUpdate.Invoke(blinkCooldown);
        blinkAvailable = false;
    }

    public void SetUltimateAvailable(bool enabled)
    {
        ultimateAvailable = enabled;
    }

    public bool IsUltimateAvailable()
    {
        return ultimateAvailable;
    }

    protected void PlaySoundAtPosition(AudioClip clip, Vector3 position, float minPitch, float maxPitch, float volume = 0.1f)
    {
        // Create a temporary AudioSource
        AudioSource tempAudioSource = new GameObject("TempAudioSource").AddComponent<AudioSource>();
        tempAudioSource.clip = clip;
        tempAudioSource.transform.position = position;
        tempAudioSource.volume = volume;

        // Randomize pitch based on the given range
        tempAudioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        tempAudioSource.Play();

        // Destroy the temporary AudioSource after it finishes playing
        Destroy(tempAudioSource.gameObject, clip.length / tempAudioSource.pitch);
    }

}