using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public AbilitiesData abilitiesData;
    public EnemiesData enemiesData;

    public GameObject enemyPrefab;

    public Dictionary<string, Ability> abilities;

    private Dictionary<Enemy, MapCoordinate> minions;

    private void Start()
    {

    }
    void Init(int minionNumber)
    {
        if(Instance != null)
        {
            Debug.LogError("More than one EnemyManager instance in scene!");
            Destroy(gameObject);
        }
        Instance = this;

        SetupAbilities();

        minions = new Dictionary<Enemy, MapCoordinate>();
        for(int i = 0; i < minionNumber; ++i)
        {
            Enemy enemy = Instantiate(enemyPrefab).AddComponent<Enemy>();
            enemy.abilities = new List<Ability>();
            enemy.currentTile = new MapCoordinate(1, 1);
            enemy.transform.position = MapManager.GetMap().GetTileObject(enemy.currentTile).transform.position
                    + Vector3.up;

            foreach(EnemiesData.EnemyData enemyData in enemiesData.enemiesData)
            {
                if(enemyData.type == EnemiesData.EnemyType.MINION)
                {
                    foreach (string abilityName in enemyData.abilityNames)
                    {
                        enemy.abilities.Add(abilities[abilityName]);
                    }
                }
            }

            minions.Add(enemy, enemy.currentTile);
        }
    }

    void SetupAbilities()
    {
        abilities = new Dictionary<string, Ability>();
        foreach(AbilitiesData.AbilityData abilityData in abilitiesData.abilities)
        {
            Ability newAbility = new Ability();
            newAbility.range = abilityData.abilityRange;
            newAbility.targetType = abilityData.targetType;
            newAbility.freeUse = abilityData.freeUse;
            newAbility.turnsCooldown = abilityData.abilityCooldown;
            newAbility.abilityEffects = new List<AbilityEffect>();
            foreach(AbilityEffect.AbilityEffectData effectData in abilityData.abilityEffects)
            {
                AbilityEffect effect = new AbilityEffect();
                effect.effect = effectData.type;
                effect.damage = effectData.damage;
                newAbility.abilityEffects.Add(effect);
            }
            abilities.Add(abilityData.abilityName, newAbility);
        }
    }
}
