using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickUp : PickUp
{
    protected override void PickMeUp(Player player)
    {
        base.PickMeUp(player);
        player.SetSpeedActive();
    }
}
