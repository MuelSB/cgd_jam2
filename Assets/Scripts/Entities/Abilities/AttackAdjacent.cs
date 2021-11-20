using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAdjacent : Ability
{
    public void Init()
    {
        targetType = AbilityTarget.ENEMY;
        range = 1;
        turnsCooldown = 1;
        freeUse = false;
    }

    public override void UseAbility(MapTile targetTile)
    {
        // Check if there is an enemy adjacent
        // If so, damage them
    }

}
