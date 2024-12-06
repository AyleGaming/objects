using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class Character : MonoBehaviour
{
    public UnityEvent<float> BlinkCoolDownUpdate;
    public static event Action<Character> OnCharacterInitialized;

    [SerializeField] protected Rigidbody2D myRigidBody;
    [SerializeField] protected float movementSpeed = 10f;
    [SerializeField] private GameObject dieEffect;
    [SerializeField] protected AudioClip collisionSound;

    public Health healthValue;
    public Weapon currentWeapon;


    public LayerMask obstacleMask;
   
    protected virtual void Start()
    {
        healthValue = new Health();
        healthValue.OnDeath.AddListener(PlayDeathEffect);
        OnCharacterInitialized?.Invoke(this);
    }

    protected virtual void Update()
    {
        
    }

    public virtual void Attack()
    {

    }

    public virtual void Look(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        myRigidBody.SetRotation(angle - 90f);
    }

    public virtual void PlayDeathEffect()
    {
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
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