using UnityEngine;

public class EnemyLaser : Enemy
{
    [SerializeField] private Transform weaponTip;
    [SerializeField] private float laserDistanceThreshold = 5f; // Distance to activate the laser
    [SerializeField] private float rotationSpeed; 
    private LineRenderer lineRenderer;

    protected override void Start()
    {
        base.Start();
        // Initialize the LineRenderer - starts disabled
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // Two points for the laser
        rotationSpeed = Random.Range(-100f, 100f);
    }
    protected override void Update()
    {
        base.Update();
        DrawLaserIfPlayerWithinDistanceThreshold();
    }

    public override void Attack()
    {
        if (attackTimer >= currentWeapon.fireRate)
        {
            currentWeapon.Shoot(weaponTip);
            attackTimer = 0;
        }
        else
        {
            attackTimer += Time.deltaTime;
        }
    }

    private void DrawLaserIfPlayerWithinDistanceThreshold()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= laserDistanceThreshold)
        {
            // Enable and update the laser line positions
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position); // Start at the enemy's position
            lineRenderer.SetPosition(1, target.transform.position);   // End at the player's position
            Attack();
        }
        else
        {
            // Disable the laser if out of range
            lineRenderer.enabled = false;
        }
    }


}
