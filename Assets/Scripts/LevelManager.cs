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
    [SerializeField]private int maxTileIntegrity = 100;
    [SerializeField]private int minTileIntegrity = 20;
    [SerializeField]private int tileXIntegrityFrequency = 3;
    [SerializeField]private int tileYIntegrityFrequency = 3;
    [SerializeField]private int tileIntegrityDivider = 10;
    [SerializeField]private Vector2Int tileDecrementRangeMaxMin = new Vector2Int(5,3);

    [SerializeField]private MapTileProperties.TileType baseBiome = MapTileProperties.TileType.Rock;
    
    //todo, serialize this!
    [SerializeField]private Dictionary<MapTileProperties.TileType, Vector2Int> biomeMaxMinStrengths = new Dictionary<MapTileProperties.TileType, Vector2Int>() {
        { MapTileProperties.TileType.Forest, new Vector2Int(20,10) },
        { MapTileProperties.TileType.Plains, new Vector2Int(25,10) },
        { MapTileProperties.TileType.Lake,   new Vector2Int(10,3)  }
    };

    [SerializeField] private Vector2Int biomeQuantityMaxMin = new Vector2Int(20,15);
    [SerializeField] private MetaDebugHeightMode debugMode = MetaDebugHeightMode.OFF;
    [SerializeField] private List<MapTileProperties.TileType> includedStructures = new List<MapTileProperties.TileType>() { 
        MapTileProperties.TileType.Forest_Village, MapTileProperties.TileType.Plains_Village, MapTileProperties.TileType.Mountain, MapTileProperties.TileType.Forest_Village_Destroyed, MapTileProperties.TileType.Plains_Village_Destroyed,
        MapTileProperties.TileType.Tower, MapTileProperties.TileType.Blood_Bog, MapTileProperties.TileType.Lighthouse, MapTileProperties.TileType.Travelling_Merchant, MapTileProperties.TileType.Shrine, MapTileProperties.TileType.Supplies,
        MapTileProperties.TileType.Ritual_Circle };

    [Header("Player")]
    [SerializeField] private GameObject playerPrefabReference;

    // Class variables
    private GameObject playerObject; // Only one player currently implemented
    private GameObject playerControllerObject; // Only one player controller currently implemented
    private PlayerController playerController; 

    private void Start()
    {
        // create the map
        CreateMap();

        // needs to be last in start
        EventSystem.Invoke(Events.LevelLoaded);

        // uncomment below to burn the world! (spooky!)
        // System.Random rand = new System.Random(seed);
        // StartCoroutine(burnTheWorld(MapManager.GetMap().GetTiles(),rand));
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

        if (tileIntegrityDivider == 0) {tileIntegrityDivider = 1;} Debug.LogWarning("tileIntegrityDivider was changed from zero to one to avoid divide by zero!");

        var metaGeneratorConfig = new MetaGeneratorConfig(seed, maxTileIntegrity, minTileIntegrity, tileXIntegrityFrequency,
            tileYIntegrityFrequency, tileIntegrityDivider, tileDecrementRangeMaxMin, baseBiome, biomeMaxMinStrengths, biomeQuantityMaxMin, debugMode, includedStructures);

        MapManager.CreateMap(mapCreateSettings, metaGeneratorConfig);
        var map = MapManager.GetMap();

        var playerStartCoordinate = new MapCoordinate(0, 0);
        var playerStartTile = map.GetTileObject(playerStartCoordinate);

        // Initialize player systems
        if(playerPrefabReference != null)
        {
            // Spawn player
            playerObject = Instantiate(playerPrefabReference, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

            // Move player onto starting tile
            var playerCollision = playerObject.GetComponent<CapsuleCollider>();
            var tileCollision = playerStartTile.GetComponent<BoxCollider>();
            var newPosition = playerStartTile.transform.position;
            newPosition.y += ((playerCollision.height * 0.5f) * playerObject.transform.localScale.y) + (tileCollision.size.y * 0.5f);
            playerObject.transform.position = newPosition;

            // Spawn player controller
            var playerControllerObject = new GameObject();
            playerController = playerControllerObject.AddComponent<PlayerController>();

            // Control player with player controller
            playerController.ControlPlayer(playerObject.GetComponent<Player>());
        }
        else
        {
            Debug.LogWarning("Player not initialized as PlayerPrefabReference was not set in the level manager.");
        }        
    }

    public IEnumerator burnTheWorld(List<GameObject> _l, System.Random _r)
    {
        while (true)
        {
            _l.ForEach(_e => _e.GetComponent<MapTile>().Decay(_r));
            yield return new WaitForSeconds(5);
        }
    }
}
