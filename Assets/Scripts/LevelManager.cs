using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class LevelManager : MonoBehaviour
{
    //Exposed Configs for in-editor value tweaking.
    [Header("Map Data")]
    [SerializeField] private GameObject mapTilePrefabReference;
    [SerializeField] private int MapWidthTileCount = 40;
    [SerializeField] private int MapDepthTileCount = 40;
    [SerializeField] private Vector3 MinRandomRotationJitter = new Vector3(-5.0f, 0.0f, -5.0f);
    [SerializeField] private Vector3 MaxRandomRotationJitter = new Vector3(-5.0f, 0.0f, -5.0f);


    [Header("Meta Generation Data")]
    [SerializeField] private int seed = 1;
    [SerializeField]private int maxTileIntegrity = 10;
    [SerializeField]private int minTileIntegrity = 1;
    [SerializeField]private int tileXIntegrityFrequency = 3;
    [SerializeField]private int tileYIntegrityFrequency = 3;
    [SerializeField]private MapTileProperties.TileType baseBiome = MapTileProperties.TileType.Rock;
    
    //todo, serialize this!
    [SerializeField]private Dictionary<MapTileProperties.TileType, Vector2Int> biomeMaxMinStrengths = new Dictionary<MapTileProperties.TileType, Vector2Int>() {
        { MapTileProperties.TileType.Forest, new Vector2Int(20,10) },
        { MapTileProperties.TileType.Plains, new Vector2Int(25,10) },
        { MapTileProperties.TileType.Lake,   new Vector2Int(10,3)  }
    };

    [SerializeField] private Vector2Int biomeQuantityMaxMin = new Vector2Int(20,15);
    [SerializeField] private MetaDebugHeightMode debugMode = MetaDebugHeightMode.OFF;

    private void Start()
    {
        // create the map
        CreateMap();

        // needs to be last in start
        EventSystem.Invoke(Events.LevelLoaded);
    }

    private void CreateMap()
    {
        // Create the game map
        var mapCreateSettings = new MapCreateSettings
        {
            MapTilePrefabReference = mapTilePrefabReference,
            MapWidthTileCount = Mathf.Clamp(MapWidthTileCount, 3, 10000),
            MapDepthTileCount = Mathf.Clamp(MapDepthTileCount, 3, 10000),
            MinRandomRotationJitter = MinRandomRotationJitter,
            MaxRandomRotationJitter = MaxRandomRotationJitter
        };

        var metaGeneratorConfig = new MetaGeneratorConfig(seed, maxTileIntegrity, minTileIntegrity, tileXIntegrityFrequency,
            tileYIntegrityFrequency, baseBiome, biomeMaxMinStrengths, biomeQuantityMaxMin, debugMode);

        MapManager.CreateMap(mapCreateSettings, metaGeneratorConfig);
    }
}
