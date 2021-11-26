using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityEffect : ScriptableObject
{
    [System.Serializable]
    public struct AbilityEffectData
    {
        public int damage;
        public EffectType type;
    };

    public enum EffectType
    {
        DAMAGE_ENTITY,
        DAMAGE_TILE,
        DESTROY_TILE,
        DESTROY_OCCUPANT,
    };

    public int damage;
    public EffectType effect;

    public virtual void ProcessEffect(MapCoordinate targetTile)
    {
        switch(effect)
        {
            case EffectType.DAMAGE_ENTITY:
                {
                    Maybe<Entity> entity = MapManager.GetMap().GetTileProperties(targetTile).tile_enitity;
                    if (entity.is_some)
                    {
                        entity.value.Damage(damage);
                    }
                    break;
                }

            case EffectType.DAMAGE_TILE:
                {
                    MapManager.GetMap().GetTileProperties(targetTile).Integrity -= damage;
                    break;
                }
            case EffectType.DESTROY_TILE:
                {
                    MapManager.GetMap().GetTileProperties(targetTile).setIntegrity(0,MapManager.GetMap().GetTileProperties(targetTile).IntegrityDivider,MapManager.GetMap().GetTileProperties(targetTile).IntegrityErosionRange);
                    break;
                }
            case EffectType.DESTROY_OCCUPANT:
                {
                    MapManager.GetMap().GetTileProperties(targetTile).Type = MapTileProperties.TileType.Unassigned;
                    break;
                }
        }
    }
}
