using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Character player;

    // DEBUG ONLY
    public Vector2 direction;
    public Vector3 mousePosition;
    public Vector3 lookDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");

        player.Move(direction);

        mousePosition = Input.mousePosition;
        // offset position Z to account for -10 position of camera in calculation
        mousePosition.z = -Camera.main.transform.position.z;

        // convert mouse to world position
        Vector3 destination = Camera.main.ScreenToWorldPoint(mousePosition);
        // Destination minus origin (player) position
        lookDirection = destination - transform.position;
        player.Look(lookDirection);

        if (Input.GetMouseButtonDown(0))
        {
            player.currentWeapon.Shoot();
        }
    }
}
