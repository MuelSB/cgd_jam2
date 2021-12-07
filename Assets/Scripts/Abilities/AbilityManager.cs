using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityEffect;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    public GameObject abilityBulletPrefab;
    public int itemPoolSize = 10;
    private List<GameObject> itemPool;
    public float bulletSpeed = 3.0f;
    public float bulletHeight = 2.0f;

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
        InitItemPool();

    }

    private void InitItemPool()
    {
        itemPool = new List<GameObject>();
        for(int i = 0; i < itemPoolSize; ++i)
        {
            GameObject go = Instantiate(abilityBulletPrefab, transform);
            go.SetActive(false);
            itemPool.Add(go);
        }
    }

    public Maybe<GameObject> GetAbilityBullet()
    {
        Maybe<GameObject> result = new Maybe<GameObject>();
        foreach (GameObject go in itemPool)
        {
            if(go.activeInHierarchy == false)
            {
                result = new Maybe<GameObject>(go);
            }
        }
        return result;
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
                effect.damageMultiplier = effectData.damageMultiplier;
                newAbility.abilityEffects.Add(effect);
            }
            abilities.Add(abilityData.abilityName, newAbility);
        }
    }

    public void HighlightTargets()
    {

    }

    public Maybe<Ability> CopyAbilityFrom(string abilityName)
    {
        Maybe<Ability> newAbility = new Maybe<Ability>();
        if (abilities.ContainsKey(abilityName) == false) return newAbility;
        Ability referenceAbility = abilities[abilityName];
        Ability ability = new Ability();

        ability.cost = referenceAbility.cost;
        ability.image = referenceAbility.image;
        ability.range = referenceAbility.range;
        ability.targetType = referenceAbility.targetType;
        ability.turnsCooldown = referenceAbility.turnsCooldown;
        ability.abilityEffects = referenceAbility.abilityEffects;

        newAbility = new Maybe<Ability>(ability);

        return newAbility;
    }

    public IEnumerator ExecuteAbility(Ability ability, Entity source, MapCoordinate targetTile)
    {
        if (targetTile != source.currentTile)
        {
            Maybe<GameObject> bullet = GetAbilityBullet();
            if (bullet.is_some)
            {
                GameObject go = bullet.value;
                go.SetActive(true);
                for (float t = 0; t < 1; t += Time.deltaTime * bulletSpeed)
                {
                    Vector3 slerped = Vector3.Slerp(source.transform.position + Vector3.up, MapManager.GetMap().GetTileObject(targetTile).transform.position + Vector3.up, t);
                    slerped.y *= (Mathf.Sin(t * Mathf.PI) * bulletHeight) + 1;

                    go.transform.position = slerped;
                    yield return 0;
                }
                go.SetActive(false);
            }
        }
        foreach (AbilityEffect effect in ability.abilityEffects)
        {
            ProcessEffect(effect, ability.baseDamage, targetTile);
            yield return 0;
        }

    }


    private void ProcessEffect(AbilityEffect effect, float baseDamage, MapCoordinate targetTile)
    {
        List<ParticleSystem> ps = new List<ParticleSystem>();
        Color color = new Color();
        switch (effect.effectType)
        {
            case EffectType.DAMAGE_ENTITY:
                {
                    color = Color.red;
                    Maybe<Entity> entity = MapManager.GetMap().GetTileProperties(targetTile).tile_enitity;
                    if (entity.is_some)
                    {
                        ps.Add(MapManager.GetMap().GetTileObject(targetTile).GetComponentInChildren<ParticleSystem>());
                        entity.value.Damage(baseDamage * effect.damageMultiplier);
                    }
                    break;
                }
            case EffectType.DAMAGE_NEIGHBOUR_ENTITIES:
                {
                    color = Color.red;
                    List<MapCoordinate> neighbours = MapManager.GetMap().GetTileNeighbors(targetTile);
                    foreach (MapCoordinate neighbour in neighbours)
                    {
                        Maybe<Entity> occupant = MapManager.GetMap().GetTileProperties(neighbour).tile_enitity;
                        if (occupant.is_some)
                        {
                            ps.Add(MapManager.GetMap().GetTileObject(neighbour).GetComponentInChildren<ParticleSystem>());
                            occupant.value.Damage(baseDamage * effect.damageMultiplier);
                        }
                    }
                    break;
                }

            case EffectType.DAMAGE_TILE:
                {
                    color = Color.black;
                    ps.Add(MapManager.GetMap().GetTileObject(targetTile).GetComponentInChildren<ParticleSystem>());
                    var mtp = MapManager.GetMap().GetTileProperties(targetTile);
                    MapManager.GetMap().GetTileProperties(targetTile).setIntegrity(mtp.Integrity - (baseDamage * effect.damageMultiplier), mtp.IntegrityDivider, mtp.IntegrityErosionRange);
                    break;
                }
            case EffectType.DESTROY_TILE:
                {
                    color = Color.black;
                    ps.Add(MapManager.GetMap().GetTileObject(targetTile).GetComponentInChildren<ParticleSystem>());
                    MapManager.GetMap().destroyTile(targetTile);
                    break;
                }
            case EffectType.DAMAGE_NEIGHBOUR_TILES:
                {
                    color = Color.black;
                    List<MapCoordinate> neighbours = MapManager.GetMap().GetTileNeighbors(targetTile);
                    foreach (MapCoordinate neighbour in neighbours)
                    {
                        ps.Add(MapManager.GetMap().GetTileObject(neighbour).GetComponentInChildren<ParticleSystem>());
                        var mtp = MapManager.GetMap().GetTileProperties(neighbour);
                        MapManager.GetMap().GetTileProperties(neighbour).setIntegrity(mtp.Integrity - (baseDamage * effect.damageMultiplier), mtp.IntegrityDivider, mtp.IntegrityErosionRange);
                    }
                    break;
                }
            case EffectType.DESTROY_SPECIAL_TILE:
                {
                    color = new Color(1.0f, 1.0f, 0.0f);
                    GameObject go = MapManager.GetMap().GetTileObject(targetTile);
                    ps.Add(go.GetComponentInChildren<ParticleSystem>());
                    SpecialTileHelper.DestroySpecialTile(go, MapManager.GetMap().getDecor(), MapManager.GetMap().getMetaSeededRandom());
                    break;
                }
            case EffectType.SPAWN_ENEMY:
                {
                    color = Color.cyan;
                    ps.Add(MapManager.GetMap().GetTileObject(targetTile).GetComponentInChildren<ParticleSystem>());
                    EnemyManager.Instance.CreateClone(EnemyManager.Instance.currentEnemyTurn, targetTile);
                    break;
                }
        }
        foreach(ParticleSystem system in ps)
        {
            var main = system.main;
            main.startColor = color;
            system.Play();
        }
    }
}
