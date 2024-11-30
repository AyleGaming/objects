using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : PickUp
{
    private readonly int startingGuns = 1;
    [SerializeField] private int gunsToAdd = 2;
    protected Player playerToGetGuns;

    protected override void PickMeUp(Player playerInTrigger)
    {
        playerToGetGuns = playerInTrigger;
        playerInTrigger.SetWingGunsActive(gunsToAdd + startingGuns);
        AudioSource.PlayClipAtPoint(pickUpAudio, playerInTrigger.transform.position, pickUpVolume);
        Destroy(gameObject);
    }
}
