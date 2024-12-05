using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Player : Character
{
    public static Player Instance { get; private set; }

    public UnityEvent<bool> OnUltimateStatusAvailable;
    [SerializeField] private Transform playerWeaponTip;
    [SerializeField] private Transform[] playerWeaponTipsArray;
    [SerializeField] private GameObject ShieldVisual;
    [SerializeField] private int gunsActive = 1;
    [SerializeField] private GameObject wingGuns;

    [SerializeField] protected AudioClip ultimateAudio;

    protected float attackTimer;
    [SerializeField] private GameObject ultimateEffectPrefab;
    [SerializeField] private Material lineMaterial;

    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    public PlayerCooldownUI cooldownUI;
    public UIManager uiManager;
    private PlayerStats playerStats;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component is missing on Player!");
        }

      
    }

    public PlayerStats Stats => playerStats;

    protected override void Start()
    {
        base.Start();
        Camera mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Get half the width and height of the sprite
            objectWidth = spriteRenderer.bounds.extents.x;
            objectHeight = spriteRenderer.bounds.extents.y;
        }
        else
        {
            Debug.LogError("No SpriteRenderer found on the Player or its children!");
        }

        healthValue.OnShieldChanged.AddListener(UpdateShieldVisual);
        UpdateShieldVisual(healthValue.GetShieldValue());
        GameObject wingGuns = GameObject.FindWithTag("WingGuns");
    }

    public override void Attack()
    {
        base.Attack();
        if (attackTimer >= currentWeapon.fireRate)
        {
            int activeGuns = Mathf.Min(gunsActive, playerWeaponTipsArray.Length);
            for (int i = 0; i < activeGuns; i++)
            {
                currentWeapon.Shoot(playerWeaponTipsArray[i]);
            }
            attackTimer = 0;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }

    protected override void Update()
    {
        base.Update();
        KeepPlayerOnScreen();

        float remainingCooldown = Mathf.Max(0, nextBlinkTime - Time.time); // Time remaining until blink is available

        if (remainingCooldown > 0)
        {
            cooldownUI.UpdateTeleportCooldown(remainingCooldown);
            uiManager.UpdateBlinkValue(remainingCooldown);
        }
        else
        {
            cooldownUI.UpdateTeleportCooldown(0f);
            uiManager.UpdateBlinkValue(0f);
            blinkAvailable = true;
        }
    }


    public override void UltimateAttack()
    {
        Debug.Log("Ultimate Attack Activated!");

        // Find all enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Assuming enemies have the "Enemy" tag

        foreach (GameObject enemyGameObject in enemies)
        {
            // Try to get the Enemy script attached to the GameObject
            Enemy enemy = enemyGameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                // Draw a line to the enemy
                StartCoroutine(DrawLineToEnemy(enemyGameObject.transform));

                // Wait for a short duration before destroying the enemy
                StartCoroutine(DestroyEnemyWithDelay(enemy, 0.5f)); // Adjust delay as needed
            }
        }

        // Play ultimate attack effect
        if (ultimateEffectPrefab != null)
        {
            Instantiate(ultimateEffectPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(ultimateAudio, transform.position, 1.0f);
        }

        // Set Ultimate status to not be available
        OnUltimateStatusAvailable.Invoke(false);
    }

    // Coroutine to draw a line to an enemy
    private IEnumerator DrawLineToEnemy(Transform enemyTransform)
    {
        // Create a new GameObject for the line
        GameObject lineObject = new ("UltimateLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Configure the LineRenderer
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        // Assign the custom material
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
        else
        {
            // Fallback material if no custom material is assigned
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // Set positions for the line
        lineRenderer.SetPosition(0, transform.position); // Start from the player's position
        lineRenderer.SetPosition(1, enemyTransform.position); // End at the enemy's position

        yield return new WaitForSeconds(0.5f); // Display the line for a short duration

        // Destroy the line object
        Destroy(lineObject);
    }

    // Coroutine to destroy an enemy with a delay
    private IEnumerator DestroyEnemyWithDelay(Enemy enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Call the PlayDeathEffect method on the Enemy
        if (enemy)
        {
            enemy.PlayDeathEffect();
        }
    }

    private void UpdateShieldVisual(float shieldValue)
    {
        if (ShieldVisual != null)
        {
            ShieldVisual.SetActive(shieldValue > 0);
        }
    }

    public void SetWingGunsActive(int gunsActiveCount)
    {
        gunsActive = gunsActiveCount;
        wingGuns.SetActive(gunsActiveCount > 1);

        if (gunsActiveCount > 1)
        {
            CancelInvoke("ResetWingGuns");
            Invoke(nameof(ResetWingGuns), 10f);
        }
        
    }

    private void KeepPlayerOnScreen()
    {
        // Clamp the player's position within the screen bounds
        float clampedX = Mathf.Clamp(transform.position.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth);
        float clampedY = Mathf.Clamp(transform.position.y, screenBounds.y * -1 + objectHeight, screenBounds.y - objectHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void ResetWingGuns()
    {
        SetWingGunsActive(1);
    }

    public override void SetUltimateAvailable(bool enabled)
    {
        base.SetUltimateAvailable(enabled);
        if (enabled)
        {
            cooldownUI.UpdateUltStatus();
        }
    }

}
