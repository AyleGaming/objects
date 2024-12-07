using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : PickUp
{
    private readonly int startingGuns = 1;
    [SerializeField] private int gunsToAdd = 2;
    
    protected override void PickMeUp(Player player)
    {
        player.SetWingGunsActive(gunsToAdd + startingGuns);
        AudioSource.PlayClipAtPoint(pickUpAudio, player.transform.position, pickUpVolume);
        Destroy(gameObject);
    }
}
