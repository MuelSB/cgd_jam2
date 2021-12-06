using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CreateEnemiesData", order = 1)]
public class EnemiesData : ScriptableObject
{
    public static EnemiesData Instance;

    public enum EnemyType
    {
        BANDIT,
        RANGER,
        TROLL,
        BAT,
        RAT,
        WOLF,
        BOSS
    }

    [System.Serializable]
    public struct EnemyData
    {
        public EnemyType type;
        public int health;
        public int movement;
        public bool canPassDestroyedTiles;
        public GameObject model;
        public List<AbilityDamage> abilities;
        public List<MapTileProperties.TileType> validTileTypes;
    }

    [System.Serializable]
    public struct AbilityDamage
    {
        public string abilityName;
        public float baseDamage;
    }

    public List<EnemyData> enemiesData;
    public EnemyData bossData;

    private void OnEnable()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Abilities data object already exists! Only one instance of this data file should exist.");
            DestroyImmediate(this);
        }
    }
}
