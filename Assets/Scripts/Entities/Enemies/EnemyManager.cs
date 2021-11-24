using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public EnemiesData enemiesData;

    public GameObject enemyPrefab;

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
            return;
        }
        Instance = this;

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
                foreach (string abilityName in enemyData.abilityNames)
                {
                    enemy.abilities.Add(AbilityManager.abilities[abilityName]);
                }
            }

            minions.Add(enemy, enemy.currentTile);
        }
    }


}
