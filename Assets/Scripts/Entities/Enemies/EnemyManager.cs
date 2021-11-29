using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public EnemiesData enemiesData;

    public GameObject enemyPrefab;

    private List<Enemy> minions;
    private Enemy boss;

    public void Init()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one EnemyManager instance in scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        minions = new List<Enemy>();
    }

    public bool CreateMinionEnemy(MapCoordinate location, EnemiesData.EnemyType type)
    {
        GameObject newMinion = Instantiate(enemyPrefab, transform);
        Enemy newEnemy = newMinion.GetComponent<Enemy>();
        newEnemy.abilities = new List<Ability>();
        newEnemy.type = Entity.EntityType.MINION;
        newEnemy.SetCurrentTile(location);
        newEnemy.transform.position = MapManager.GetMap().GetTileObject(newEnemy.currentTile).transform.position
                + Vector3.up;
        minions.Add(newMinion.GetComponent<Enemy>());


        foreach (EnemiesData.EnemyData enemyData in enemiesData.enemiesData)
        {
            if (enemyData.type == type)
            {
                foreach (string abilityName in enemyData.abilityNames)
                {
                    newEnemy.abilities.Add(AbilityManager.abilities[abilityName]);
                }
                newEnemy.movementRange = enemyData.movement;
                newEnemy.health = enemyData.health;
            }
        }

        return true;
    }

    public bool CreateBoss(MapCoordinate location)
    {

        return false;
    }
}
