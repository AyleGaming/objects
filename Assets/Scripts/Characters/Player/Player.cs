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

    protected int blinkDistance = 1;
    protected float blinkCooldown = 10f;
    protected float nextBlinkTime = 0f;
    protected bool isBlinking = false;
    public bool blinkAvailable = true;
    [SerializeField] protected AudioClip blinkSound;
    [SerializeField] private GameObject blinkPrefab; // Assign your portal prefab in the Inspector
    [SerializeField] private float blinkDelay = 0.5f; // Time before the player blinks to the position
    protected bool checkForCollisions = false;

    protected float attackTimer;

    [SerializeField] private GameObject wingGuns;
    [SerializeField] protected AudioClip ultimateAudio;
    [SerializeField] public bool ultimateAvailable = false;
    [SerializeField] private GameObject ultimateEffectPrefab;
    [SerializeField] private Material lineMaterial;

    // Lock player on screen
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
        attackTimer += Time.deltaTime;
    }

    public override void Attack()
    {
        base.Attack();
        float newAttackSpeed = playerStats.GetStat("attackSpeed");

        if (attackTimer >= newAttackSpeed)
        {
            int gunsActive = playerStats.GetStatInt("gunsActive");
            float additiveWeaponDamage = playerStats.GetStat("additiveWeaponDamage");

            int activeGuns = Mathf.Min(gunsActive, playerWeaponTipsArray.Length);
            for (int i = 0; i < activeGuns; i++)
            {
                currentWeapon.Shoot(playerWeaponTipsArray[i], additiveWeaponDamage);
            }
            attackTimer = 0;
        }
    }


    public virtual void Move(Vector2 direction)
    {
        if (isBlinking) return;
        float moveSpeed = playerStats.GetStat("movementSpeed");
        myRigidBody.AddForce(moveSpeed * Time.deltaTime * direction, ForceMode2D.Impulse);
    }


    private void UpdateShieldVisual(float shieldValue)
    {
        if (ShieldVisual != null)
        {
            ShieldVisual.SetActive(shieldValue > 0);
        }
    }
    

    private void KeepPlayerOnScreen()
    {
        // Clamp the player's position within the screen bounds
        float clampedX = Mathf.Clamp(transform.position.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth);
        float clampedY = Mathf.Clamp(transform.position.y, screenBounds.y * -1 + objectHeight, screenBounds.y - objectHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }


 
    /**********************************
     * 
     * 
     * ABILITIES 
     * 
     * 
     *********************************/


    /**********************************
    * 
    * BLINK
    * 
    *********************************/
    
    public virtual void Blink(Vector3 lookDirection)
    {
        if (blinkAvailable && playerStats.GetStatBool("hasBlinkAbility") == true)
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
            StartCoroutine(BlinkWithDelay(targetPosition));
            PlaySoundAtPosition(blinkSound, transform.position, 1f, 1f);
        }
    }

    private IEnumerator BlinkWithDelay(Vector3 targetPosition)
    {
        // Step 1: Spawn the portal icon at the target position
        GameObject blink = Instantiate(blinkPrefab, targetPosition, Quaternion.identity);

        // Step 2: Fade in the portal (if applicable)
        Blink portalScript = blink.GetComponent<Blink>();
        if (portalScript != null)
        {
            portalScript.FadeIn();
        }

        // Step 3: Wait for the delay
        yield return new WaitForSeconds(blinkDelay);

        // Step 4: Move the player to the target position
        myRigidBody.MovePosition(targetPosition);

        // Optional: Destroy the portal icon after teleportation
        Destroy(blink);

        // Re-enable movement after blink
        Invoke(nameof(EndBlink), 0.1f);
    }

 

    private void EndBlink()
    {
        isBlinking = false; // Re-enable movement
        nextBlinkTime = Time.time + blinkCooldown;
        BlinkCoolDownUpdate.Invoke(blinkCooldown);
        blinkAvailable = false;
    }

    /**********************************
    * 
    * ULTIMATE 
    * 
    *********************************/

    public void UltimateAttack()
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
        GameObject lineObject = new("UltimateLine");
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

    public virtual void SetUltimateAvailable(bool enabled)
    {
        ultimateAvailable = enabled;
        if (ultimateAvailable)
        {
            cooldownUI.UpdateUltStatus();
        }
    }


    public bool IsUltimateAvailable()
    {
        return playerStats.GetStatBool("hasUltimateAbility") && ultimateAvailable == true;
    }


    /**********************************
    * 
    * ADDITIONAL GUNS 
    * 
    *********************************/

    public void SetWingGunsActive(int gunsActiveCount)
    {
        gunsActive = gunsActiveCount;
        // set guns active to number of guns
        playerStats.SetStatInt("gunsActive", gunsActive);
        
        // Show UI
        wingGuns.SetActive(gunsActiveCount > 1);

        if (gunsActiveCount > 1)
        {
            CancelInvoke("ResetWingGuns");
            float additionalGunsTime = playerStats.GetStat("additionalGunsTime");
            Invoke(nameof(ResetWingGuns), additionalGunsTime);
            float newAdditionalGunsTime = additionalGunsTime + .5f;
            playerStats.SetStat("additionalGunsTime", newAdditionalGunsTime);
        }
    }

    private void ResetWingGuns()
    {
        SetWingGunsActive(playerStats.GetStatInt("gunsBase"));
    }


    /**********************************
    * 
    * SPEED
    * 
    *********************************/

    public void SetSpeedActive()
    {
        float newAttackSpeed = playerStats.GetStat("attackSpeed") * .8f;
        float newMovementSpeed = playerStats.GetStat("movementSpeed") * 1.25f;
        float newSpeedBuffDuration = playerStats.GetStat("speedBuffDuration") + .5f;

        playerStats.SetStat("attackSpeed", newAttackSpeed);
        playerStats.SetStat("movementSpeed", newMovementSpeed);
        playerStats.SetStat("speedBuffDuration", newSpeedBuffDuration);

        CancelInvoke("ResetSpeed");
        Invoke(nameof(ResetSpeed), newSpeedBuffDuration);
    }

    private void ResetSpeed()
    {
        // reset attack speed and movement speed but not buff duration
        playerStats.SetStat("attackSpeed", .30f);
        playerStats.SetStat("movementSpeed", 18f);
    }

}
