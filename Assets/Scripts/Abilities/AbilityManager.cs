using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityEffect;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    public static Dictionary<string, Ability> abilities;
    public AbilitiesData abilitiesData;

    public void Init()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one AbilityManager instance in scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetupAbilities();
    }

    void SetupAbilities()
    {
        abilities = new Dictionary<string, Ability>();
        foreach (AbilitiesData.AbilityData abilityData in abilitiesData.abilities)
        {
            Ability newAbility = ScriptableObject.CreateInstance<Ability>();
            newAbility.range = abilityData.abilityRange;
            newAbility.targetType = abilityData.targetType;
            newAbility.cost = abilityData.cost;
            newAbility.turnsCooldown = abilityData.abilityCooldown;
            newAbility.abilityEffects = new List<AbilityEffect>();
            foreach (AbilityEffect.AbilityEffectData effectData in abilityData.abilityEffects)
            {
                AbilityEffect effect = new AbilityEffect();
                effect.effectType = effectData.type;
                effect.damage = effectData.damage;
                newAbility.abilityEffects.Add(effect);
            }
            abilities.Add(abilityData.abilityName, newAbility);
        }
    }

    public void HighlightTargets()
    {

    }

    public IEnumerator ExecuteAbility(Ability ability, MapCoordinate targetTile)
    {
        foreach (AbilityEffect effect in ability.abilityEffects)
        {
            ProcessEffect(effect, targetTile);
            yield return 0;
        }

    }


    private void ProcessEffect(AbilityEffect effect, MapCoordinate targetTile)
    {
        switch (effect.effectType)
        {
            case EffectType.DAMAGE_ENTITY:
                {
                    Maybe<Entity> entity = MapManager.GetMap().GetTileProperties(targetTile).tile_enitity;
                    if (entity.is_some)
                    {
                        entity.value.Damage(effect.damage);
                    }
                    break;
                }
            case EffectType.DAMAGE_NEIGHBOUR_ENTITIES:
                {
                    List<MapCoordinate> neighbours = MapManager.GetMap().GetTileNeighbors(targetTile);
                    foreach (MapCoordinate neighbour in neighbours)
                    {
                        Maybe<Entity> occupant = MapManager.GetMap().GetTileProperties(neighbour).tile_enitity;
                        if (occupant.is_some)
                        {
                            occupant.value.Damage(effect.damage);
                        }
                    }
                    break;
                }

            case EffectType.DAMAGE_TILE:
                {
                    var mtp = MapManager.GetMap().GetTileProperties(targetTile);
                    MapManager.GetMap().GetTileProperties(targetTile).setIntegrity(mtp.Integrity - effect.damage, mtp.IntegrityDivider, mtp.IntegrityErosionRange);
                    break;
                }
            case EffectType.DESTROY_TILE:
                {
                    MapManager.GetMap().destroyTile(targetTile);
                    break;
                }
            case EffectType.DAMAGE_NEIGHBOUR_TILES:
                {
                    List<MapCoordinate> neighbours = MapManager.GetMap().GetTileNeighbors(targetTile);
                    foreach (MapCoordinate neighbour in neighbours)
                    {
                        var mtp = MapManager.GetMap().GetTileProperties(neighbour);
                        MapManager.GetMap().GetTileProperties(neighbour).setIntegrity(mtp.Integrity - effect.damage, mtp.IntegrityDivider, mtp.IntegrityErosionRange);
                    }
                    break;
                }
            case EffectType.DESTROY_SPECIAL_TILE:
                {
                    MetaGeneratorHelper.destroyTile(MapManager.GetMap(), targetTile);
                    break;
                }
            case EffectType.SPAWN_ENEMY:
                {
                    EnemyManager.Instance.CreateClone(EnemyManager.Instance.currentEnemyTurn);
                    break;
                }
        }
    }
}
