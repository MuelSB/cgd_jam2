using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public enum AbilityTarget
    {
        ENEMY,
        ALLY,
        TILE
    }

    public int range;

    protected AbilityTarget targetType;

    // 0 turns cooldown means it's only usable once per session
    protected int turnsCooldown;

    // Can it be used alongside other abilities? Ie. a 'Free action'
    public bool freeUse;

    public List<AbilityEffect> abilityEffects;

    public void UseAbility(MapCoordinate targetTile)
    {
        foreach(AbilityEffect effect in abilityEffects)
        {
            effect.ProcessEffect(targetTile);
        }
    }

    public virtual MapCoordinate CanUseAbility(MapCoordinate currentTile)
    {
        return currentTile;
    }
}
