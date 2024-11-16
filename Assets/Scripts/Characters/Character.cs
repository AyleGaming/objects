using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private Rigidbody2D myRigidBody;
    [SerializeField] private float movementSpeed = 10f;

    public Health healthValue;
    public Weapon currentWeapon;

    protected virtual void Start()
    {
        healthValue = new Health(3);
        healthValue.OnDeath.AddListener(PlayDeathEffect);
    }

    public virtual void Move(Vector2 direction)
    {
        myRigidBody.AddForce(direction * Time.deltaTime * movementSpeed, ForceMode2D.Impulse);
    }

    public virtual void PlayDeathEffect()
    {
        Destroy(gameObject);
    }

    public virtual void Look(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //angle = Vector2.Angle(Vector2.right, direction);

        myRigidBody.SetRotation(angle -90f);
    }

    public void Interact()
    {

    }

    public virtual void Attack()
    {
        Debug.Log("character punch");
    }
}