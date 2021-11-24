using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AbilityEffect
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
        DAMAGE_NEIGHBOUR_ENTITIES,
        DAMAGE_TILE,
        DESTROY_TILE,
        DAMAGE_NEIGHBOUR_TILES,
        DESTROY_SPECIAL_TILE,
        TELEPORT,
        RANDOM_TELEPORT,
        KNOCKBACK,
        SPAWN_ENEMY,
    };

    public int damage;
    public EffectType effectType;
}
