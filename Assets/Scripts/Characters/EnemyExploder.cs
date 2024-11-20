using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExploder : Enemy
{
    [SerializeField] private bool hasCollided = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if(hasCollided == true)
        {
            Attack();
        }
    }

    public override void Attack()
    {
        target.healthValue.DecreaseHealth(3);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Triggered");
            hasCollided = true;
        }
    }
}
