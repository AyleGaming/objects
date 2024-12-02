using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickUp : PickUp
{
    [SerializeField] private int shieldPointsToAdd;

    protected override void PickMeUp(Player player)
    {
        player.healthValue.IncreaseShield(shieldPointsToAdd);
        AudioSource.PlayClipAtPoint(pickUpAudio, player.transform.position, pickUpVolume);
        Destroy(gameObject);
    }
}
