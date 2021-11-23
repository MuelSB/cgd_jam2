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

    private Dictionary<Enemy, Vector2> minions;
    private Boss boss;

    bool inited = false;

    private void Update()
    {
        if (MapManager.GetMap() != null && inited == false && Input.GetKeyDown(KeyCode.Space))
        {
            Init(1);
            inited = true;
        }
    }

    private void Start()
    {

    }
    void Init(int starterMinions)
    {
        if(Instance != null)
        {
            Debug.LogError("More than one EnemyManager instance in scene!");
            Destroy(gameObject);
        }
        Instance = this;

        SetupAbilities();

        minions = new Dictionary<Enemy, Vector2>();
        for(int i = 0; i < starterMinions; ++i)
        {
            Enemy enemy = Instantiate(enemyPrefab).AddComponent<Enemy>();
            enemy.abilities = new List<Ability>();
            enemy.currentTile = new MapCoordinate(1, 1);
            enemy.transform.position = MapManager.GetMap().GetTileObject(enemy.currentTile).transform.position;

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

            minions.Add(enemy, Vector2.zero);
        }
        EnemyTurn();
    }

    public void EnemyTurn()
    {
        foreach(var enemy in minions)
        {
            enemy.Key.ProcessTurn();
        }
    }

    void SetupAbilities()
    {
        abilities = new Dictionary<string, Ability>();
        foreach(AbilitiesData.AbilityData abilityData in abilitiesData.abilities)
        {
            Ability newAbility = new Ability();
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
