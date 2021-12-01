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

    [HideInInspector]
    public Enemy currentEnemyTurn;

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

    public void CreateClone(Enemy enemy)
    {
        foreach(EnemiesData.EnemyData enemyData in enemiesData.enemiesData)
        {
            if(enemyData.type == enemy.enemyType)
            {
                List<MapCoordinate> possibleTiles = MapManager.GetMap().GetTileNeighbors(enemy.currentTile);
                List<MapCoordinate> emptyTiles = new List<MapCoordinate>();
                foreach(MapCoordinate tile in possibleTiles)
                {
                    if(MapManager.GetMap().GetTileProperties(tile).tile_enitity.is_some == false)
                    {
                        emptyTiles.Add(tile);
                    }
                }
                CreateEnemyOfType(emptyTiles[Random.Range(0, emptyTiles.Count)], enemyData);
            }
        }
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
            int pickedEnemy = Random.Range(0, potentialSpawn.Count);
            EnemiesData.EnemyData enemyData = potentialSpawn[pickedEnemy];

            minions.Add(CreateEnemyOfType(location, enemyData));
            return true;
        }

        return false;
    }

    public bool CreateBoss()
    {
        int maxX, maxY;
        maxX = MapManager.GetMap().GetWidthTileCount() - 1;
        maxY = MapManager.GetMap().GetDepthTileCount() - 1;
        MapCoordinate bossStart = new MapCoordinate(maxX, maxY);
        boss = CreateEnemyOfType(bossStart, enemiesData.bossData);
        boss.entityType = Entity.EntityType.BOSS;
        return true;
    }

    private Enemy CreateEnemyOfType(MapCoordinate location, EnemiesData.EnemyData enemyData)
    {
        GameObject newEnemyObject = Instantiate(enemyPrefab, transform);
        Enemy newEnemy = newEnemyObject.GetComponent<Enemy>();
        newEnemy.abilities = new List<Ability>();
        newEnemy.entityType = Entity.EntityType.MINION;
        newEnemy.enemyType = enemyData.type;
        newEnemy.canPassDestroyedTiles = enemyData.canPassDestroyedTiles;
        newEnemy.SetCurrentTile(location);
        newEnemy.transform.position = MapManager.GetMap().GetTileObject(newEnemy.currentTile).transform.position
                + Vector3.up;

        foreach (string abilityName in enemyData.abilityNames)
        {
            newEnemy.abilities.Add(AbilityManager.abilities[abilityName]);
        }
        newEnemy.movementRange = enemyData.movement;
        newEnemy.health = enemyData.health;
        newEnemyObject.name = enemyData.type.ToString();
        return newEnemy;
    }

    public bool CreateBoss(MapCoordinate location)
    {

        return false;
    }
}
