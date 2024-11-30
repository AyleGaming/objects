using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickUp : PickUp
{
    [SerializeField] private int shieldPointsToAdd;

    protected override void PickMeUp(Player playerInTrigger)
    {
        playerInTrigger.healthValue.IncreaseShield(shieldPointsToAdd);
        AudioSource.PlayClipAtPoint(pickUpAudio, playerInTrigger.transform.position, pickUpVolume);
        Destroy(gameObject);
    }
}
