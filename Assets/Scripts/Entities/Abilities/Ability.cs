using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public enum AbilityTarget
    {
        ENEMY,
        ALLY,
        TILE
    }

    public int range;
    protected float baseDamage;

    protected AbilityTarget targetType;

    // 0 turns cooldown means it's only usable once per session
    protected int turnsCooldown;

    // Can it be used alongside other abilities? Ie. a 'Free action'
    public bool freeUse;

    public abstract void UseAbility(MapTile targetTile);

    public virtual MapTile CanUseAbility(MapTile currentTile)
    {
        // Check if there is a valid tile in range
        return null;
    }
}
