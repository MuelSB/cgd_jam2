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

    public void SpawnNewEnemies(bool pauseForEach)
    {
        int numToCreate = Random.Range(5, 10);
        StartCoroutine(CreateNewEnemies(numToCreate, pauseForEach));
    }

    public IEnumerator CreateNewEnemies(int numEnemies, bool pauseForEach)
    {
        for (int i = 0; i < numEnemies; ++i)
        {
            bool success = false;
            while (success == false)
            {
                var playerLoc = MetaGeneratorHelper.getPlayerFromGrid(MapManager.GetMap().GetTiles());
                if (playerLoc.is_some)
                {
                    success = CreateMinionEnemy(FindEnemySpawnLocation(playerLoc.value));
                }
            }
            if (pauseForEach)
            {
                yield return 0;
            }
        }
        yield break;
    }

    private MapCoordinate FindEnemySpawnLocation(MapCoordinate playerLocation)
    {
        int playerRange = 5;

        int minMaxX = Random.Range(0, 2);
        int minMaxY = Random.Range(0, 2);

        int maxX = MapManager.GetMap().GetWidthTileCount()-1;
        int maxY = MapManager.GetMap().GetDepthTileCount()-1;

        int spawnX = ((minMaxX % 2 == 0 && playerLocation.x > playerRange) || playerLocation.x > (maxX - playerRange)) ?
                        Random.Range(0, playerLocation.x - playerRange)
                        : Random.Range(playerLocation.x + playerRange, maxX);

        int spawnY = ((minMaxY % 2 == 0 && playerLocation.y > playerRange) || playerLocation.y > (maxY - playerRange)) ?
                Random.Range(0, playerLocation.y - playerRange)
                : Random.Range(playerLocation.y + playerRange, maxY);

        return new MapCoordinate(spawnX, spawnY);
    }

    public bool CreateMinionEnemy(MapCoordinate location)
    {

        MapTileProperties.TileType tileType = MapManager.GetMap().GetTileProperties(location).Type;
        List<EnemiesData.EnemyData> potentialSpawn = new List<EnemiesData.EnemyData>();

        foreach (EnemiesData.EnemyData enemyData in enemiesData.enemiesData)
        {
            foreach (MapTileProperties.TileType possibleTile in enemyData.validTileTypes)
            {
                if(enemyData.validTileTypes.Contains(tileType))
                {
                    potentialSpawn.Add(enemyData);
                }
            }
        }

        if(potentialSpawn.Count > 0)
        {
            GameObject newMinion = Instantiate(enemyPrefab, transform);
            Enemy newEnemy = newMinion.GetComponent<Enemy>();
            newEnemy.abilities = new List<Ability>();
            newEnemy.type = Entity.EntityType.MINION;
            newEnemy.SetCurrentTile(location);
            newEnemy.transform.position = MapManager.GetMap().GetTileObject(newEnemy.currentTile).transform.position
                    + Vector3.up;
            minions.Add(newMinion.GetComponent<Enemy>());

            int pickedEnemy = Random.Range(0, potentialSpawn.Count);
            EnemiesData.EnemyData enemyData = potentialSpawn[pickedEnemy];

            foreach (string abilityName in enemyData.abilityNames)
            {
                newEnemy.abilities.Add(AbilityManager.abilities[abilityName]);
            }
            newEnemy.movementRange = enemyData.movement;
            newEnemy.health = enemyData.health;
            newMinion.name = enemyData.type.ToString();
            return true;
        }

        return false;
    }

    public bool CreateBoss(MapCoordinate location)
    {

        return false;
    }
}
