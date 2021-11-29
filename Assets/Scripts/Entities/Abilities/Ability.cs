using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public enum AbilityTarget
    {
        ENTITY,
        TILE,
        BUILDING
    }

    public int range;

    public AbilityTarget targetType;

    // 0 turns cooldown means it's only usable once per session
    public int turnsCooldown;

    // AP cost of ability - this only applies to the player
    public int cost;

    public List<AbilityEffect> abilityEffects;

    public List<MapCoordinate> GetTilesInRange(MapCoordinate currentTile)
    {
        List<MapCoordinate> potentialTargets = new List<MapCoordinate>();

        for (var i = range; i >= -range; --i)
        {
            var yMax = range - Mathf.Abs(i);
            for (var j = yMax; j >= -yMax; --j)
            {
                MapCoordinate newTarget = new MapCoordinate(currentTile.x + i, currentTile.y + j);

                if(MapManager.GetMap().IsValidCoordinate(newTarget))
                {
                    potentialTargets.Add(new MapCoordinate(currentTile.x + i, currentTile.y + j));
                }
            }
        }

        return potentialTargets;
    }

    public List<MapCoordinate> GetTargetableTiles(MapCoordinate currentTile)
    {
        List<MapCoordinate> tiles = GetTilesInRange(currentTile);
        List<MapCoordinate> targets = new List<MapCoordinate>();

        foreach(MapCoordinate coord in tiles)
        {
            MapTileProperties properties = MapManager.GetMap().GetTileProperties(coord);

            switch(targetType)
            {
                case AbilityTarget.ENTITY:
                    {
                        if(properties.tile_enitity.is_some && coord != currentTile)
                        {
                            targets.Add(coord);
                        }
                        break;
                    }
                case AbilityTarget.TILE:
                    {
                        if(properties.Integrity > 0)
                        {
                            targets.Add(coord);
                        }
                        break;
                    }
                case AbilityTarget.BUILDING:
                    {
                        // check if type is a building
                        break;
                    }
            }
        }

        return targets;
    }
}
