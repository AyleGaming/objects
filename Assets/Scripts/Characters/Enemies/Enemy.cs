using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float distanceToStop = 3f;
    [SerializeField] private float attackCooldown = 3f;

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private bool isMiniboss;
    [SerializeField] private bool isBoss;
    [SerializeField] private AudioClip attackSound; // Sounds for attack
    [SerializeField] private int attackDamage = 3; 

    protected Player target;
    protected float attackTimer;

    private EnemyVariant enemyVariantScript;

    void Awake()
    {
        enemyVariantScript = GetComponent<EnemyVariant>();
    }

    protected override void Start()
    {
        base.Start();

        // Set Enemy Health
        healthValue.SetHealthValue(80);
        healthValue.SetShieldValue(0);
        target = FindObjectOfType<Player>();

        // Check if the script is null and log a message for debugging
        if (enemyVariantScript == null)
        {
            Debug.LogError("EnemyVariant is null in SetupEnemy for " + gameObject.name);
        }
    }

    public void SetupEnemy(EnemyType enemyType, int variant)
    {
        if (enemyVariantScript == null)
        {
            Debug.LogError("EnemyVariant is null in SetupEnemy for " + gameObject.name);
        }

        // Set the sprite based on the EnemyType and VariantID
        enemyVariantScript.LoadSpriteBasedOnEnemyType(enemyType, variant);
    }

    protected override void Update()
    {
        // dead?
        if (target == null) return;

        Vector2 destination = target.transform.position;
        Vector2 currentPosition = transform.position;
        Vector2 direction = destination - currentPosition;

        if(Vector2.Distance(destination, currentPosition) > distanceToStop)
        {
            Move(direction.normalized);
        } else
        {
            Attack();
        }
        Look(direction.normalized);
    }

    public override void Attack()
    {
        base.Attack();
        if(attackTimer >= attackCooldown)
        {
            if (attackSound != null)
            {
                PlaySoundAtPosition(attackSound, transform.position, 1.2f, 1.4f);
            }
            target.healthValue.DecreaseHealth(attackDamage);
            attackTimer = 0;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }

    public override void PlayDeathEffect()
    {
        GameManager.Instance.RemoveEnemyFromList(this);
        base.PlayDeathEffect();
        if (collisionSound != null)
        {
            PlaySoundAtPosition(collisionSound, transform.position, .9f, 1.1f);
        }
    }
}
