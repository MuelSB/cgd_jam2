using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public enum AbilityTarget
    {
        ENEMY,
        ALLY,
        TILE,
        BUILDING
    }

    public Sprite image = null;
    
    public int range;
    
    public AbilityTarget targetType;

    // 0 turns cooldown means it's only usable once per session
    public int turnsCooldown;

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

    public virtual List<MapCoordinate> CanUseAbility(MapCoordinate currentTile)
    {
        List<MapCoordinate> potentialTargets = new List<MapCoordinate>();
        if(targetType == AbilityTarget.TILE)
        {
            potentialTargets = MapManager.GetMap().GetTileNeighbors(currentTile);
        }
        else if(targetType == AbilityTarget.ENEMY)
        {
            foreach(MapCoordinate tile in MapManager.GetMap().GetTileNeighbors(currentTile))
            {
                if(MapManager.GetMap().GetTileProperties(tile).ContainsEntity)
                {
                    potentialTargets.Add(tile);
                }
            }
        }
        return potentialTargets;
    }
}
