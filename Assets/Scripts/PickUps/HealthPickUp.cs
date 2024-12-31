using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : PickUp
{
    [SerializeField] private int healthPointsToAdd;

    protected override void PickMeUp(Player player)
    {
        base.PickMeUp(player);
        player.healthValue.IncreaseHealth(healthPointsToAdd);
    }
}
