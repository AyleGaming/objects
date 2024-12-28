using UnityEngine;

public class StartScreenInput : MonoBehaviour
{
	[SerializeField] private Rigidbody2D myRigidBody;

	// DEBUG ONLY
	public Vector2 direction;
	public Vector3 mousePosition;
	public Vector3 lookDirection;

	public float moveSpeed = 15f;
	protected float attackTimer;
	[SerializeField] private Transform playerWeaponTip;
	public Weapon currentWeapon;

	// Lock player on screen
	private Vector2 screenBounds;
	private float objectWidth;
	private float objectHeight;


	private void Start()
    {
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
	}

	void Update()
	{
		direction.x = Input.GetAxis("Horizontal");
		direction.y = Input.GetAxis("Vertical");

		Move(direction);

		mousePosition = Input.mousePosition;
		// offset position Z to account for -10 position of camera in calculation
		mousePosition.z = -Camera.main.transform.position.z;

		// convert mouse to world position
		Vector3 destination = Camera.main.ScreenToWorldPoint(mousePosition);
		// Destination minus origin (player) position
		lookDirection = destination - transform.position;
		Look(lookDirection);


		if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
		{
			Attack();
		}

		KeepPlayerOnScreen();
		attackTimer += Time.deltaTime;
	}

	private void Look(Vector2 direction)
	{
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		myRigidBody.SetRotation(angle - 90f);
	}

	private  void Move(Vector2 direction)
	{
		myRigidBody.AddForce(moveSpeed * Time.deltaTime * direction, ForceMode2D.Impulse);
	}

	private void KeepPlayerOnScreen()
	{
		// Clamp the player's position within the screen bounds
		float clampedX = Mathf.Clamp(transform.position.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth);
		float clampedY = Mathf.Clamp(transform.position.y, screenBounds.y * -1 + objectHeight, screenBounds.y - objectHeight);

		transform.position = new Vector3(clampedX, clampedY, transform.position.z);
	}

	private void Attack()
	{
		float attackSpeed = .35f;
		float additiveWeaponDamage = 1f;

		if (attackTimer >= attackSpeed)
		{
			currentWeapon.Shoot(playerWeaponTip, additiveWeaponDamage);
			attackTimer = 0;
		}
		
	}

}
