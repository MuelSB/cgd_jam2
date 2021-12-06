using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Map
{
    // Class variables
    private int mapWidthTileCount;
    private int mapDepthTileCount;
    private float mapTileSize;
    private List<GameObject> mapTiles = new List<GameObject>();
    private bool instantiated = false;

    //class used for generation of map information
    private MetaGenerator metaGenerator;

    public Map(MapCreateSettings createSettings, MetaGeneratorConfig metaGeneratorConfig, Transform parentTransform)
    {
        Destroy();
        MakeMapMetaGenerator(metaGeneratorConfig);
        Instantiate(createSettings, parentTransform);
    }

    //Should only need to be made once at the start, so it only makes it if it does not exist already.
    public void MakeMapMetaGenerator(MetaGeneratorConfig _config) {
        if (metaGenerator == null) {
            metaGenerator = new MetaGenerator(this,_config);
        }
    }

    public void Destroy()
    {
        // Destroy all tile instances
        for (int i = 0; i < mapTiles.Count; ++i)
        {
            GameObject.Destroy(mapTiles[i]);
        }

        // Reset class variables
        mapWidthTileCount = 0;
        mapDepthTileCount = 0;

        instantiated = false;
    }

    public ulong AddEventToTile(MapTileEvent ev, MapCoordinate coordinate)
    {
        return GetTileProperties(coordinate).AddEvent(ev);
    }

    public List<ulong> AddEventToAllTiles(MapTileEvent ev)
    {
        List<ulong> ids = new List<ulong>();

        foreach(GameObject tileObject in mapTiles)
        {
            ids.Add(tileObject.GetComponent<MapTile>().GetProperties().AddEvent(ev));
        }

        return ids;
    }

    public void RemoveEventFromTile(ulong eventID, MapCoordinate coordinate)
    {
        GetTileProperties(coordinate).RemoveEvent(eventID);
    }

    public void ActivateTile(MapCoordinate coordinate)
    {
        GetTileProperties(coordinate).ActivateEvents();
    }

    public void ActivateAllTiles()
    {
        foreach (GameObject tileObject in mapTiles)
        {
            tileObject.GetComponent<MapTile>().GetProperties().ActivateEvents();
        }
    }

    public void RemoveAllTileEvents()
    {
        foreach (GameObject tileObject in mapTiles)
        {
            tileObject.GetComponent<MapTile>().GetProperties().ClearEvents();
        }
    }

    public List<GameObject> GetTiles() => mapTiles; 

    public float GetTileSize() { return mapTileSize; }

    // Retrieves the tile game object at map coordinate. Map coordinates map to 0, 0 being the top left tile when looking at the grid along
    // the negative X axis
    public GameObject GetTileObject(MapCoordinate mapCoordinate)
    {
        Assert.IsTrue(IsValidCoordinate(mapCoordinate), "Attempting to access map with invalid grid coordinate.");
        return mapTiles[mapCoordinate.y * mapWidthTileCount + mapCoordinate.x];
    }

    // Retrieves the properties of the tile at map coordinate. Map coordinates map to 0, 0 being the top left tile when looking at the grid along
    // the negative X axis
    public MapTileProperties GetTileProperties(MapCoordinate mapCoordinate)
    {
        Assert.IsTrue(IsValidCoordinate(mapCoordinate), "Attempting to access map with invalid grid coordinate.");
        return GetTile(mapCoordinate).GetProperties();
    }

    // Retrieves the tile at map coordinate. Map coordinates map to 0, 0 being the top left tile when looking at the grid along
    // the negative X axis
    public MapTile GetTile(MapCoordinate mapCoordinate)
    {
        Assert.IsTrue(IsValidCoordinate(mapCoordinate), "Attempting to access map with invalid grid coordinate.");
        return GetTileObject(mapCoordinate).GetComponent<MapTile>();
    }

    // Returns the coordinates of the neighbors of the tile at tileMapCoordinate
    public List<MapCoordinate> GetTileNeighbors(MapCoordinate tileMapCoordinate)
    {
        Assert.IsTrue(IsValidCoordinate(tileMapCoordinate), "Attempting to access map with invalid grid coordinate.");

        List<MapCoordinate> neighbors = new List<MapCoordinate>();

        if((tileMapCoordinate.y - 1) >= 0)
        {
            neighbors.Add(new MapCoordinate(tileMapCoordinate.x, tileMapCoordinate.y - 1));
        }

        if ((tileMapCoordinate.x + 1) < mapWidthTileCount)
        {
            neighbors.Add(new MapCoordinate(tileMapCoordinate.x + 1, tileMapCoordinate.y));
        }

        if ((tileMapCoordinate.x - 1) >= 0)
        {
            neighbors.Add(new MapCoordinate(tileMapCoordinate.x - 1, tileMapCoordinate.y));
        }

        if (((tileMapCoordinate.y + 1) < mapDepthTileCount))
        {
            neighbors.Add(new MapCoordinate(tileMapCoordinate.x, tileMapCoordinate.y + 1));
        }

        return neighbors;
    }

    public bool IsValidCoordinate(MapCoordinate mapCoordinate)
    {
        return mapCoordinate.x >= 0 &&
            mapCoordinate.y >= 0 &&
            mapCoordinate.x < mapWidthTileCount &&
            mapCoordinate.y < mapDepthTileCount;
    }

    private void Instantiate(MapCreateSettings createSettings, Transform parentTransform)
    {
        Assert.IsFalse(instantiated, "Attempting to instantiate the map without destroying existing map data.");

        // Compute tile properties
        Assert.IsTrue(createSettings.MapTilePrefabReference.transform.localScale.x == createSettings.MapTilePrefabReference.transform.localScale.z,
            "Only square tiles are currently supported.");

        mapTileSize = createSettings.MapTilePrefabReference.transform.localScale.x*5;

        var mapWidth = mapTileSize * createSettings.MapWidthTileCount;
        var mapDepth = mapTileSize * createSettings.MapDepthTileCount;

        Assert.IsTrue(mapWidth > 0 && mapDepth > 0, "Attempting to instantiate a map with a 0 side. A map must have a tile width and depth of at least 1.");

        // For each row in the map
        for (int i = 0; i < createSettings.MapDepthTileCount; ++i)
        {
            // For each column in the map
            for (int j = 0; j < createSettings.MapWidthTileCount; ++j)
            {
                // Instantiate an instance of the tile prefab at the correct location in the grid
                // Each tile is offset to make the center of the map grid 0, 0, 0
                mapTiles.Add(
                    GameObject.Instantiate(
                        createSettings.MapTilePrefabReference,
                        new Vector3(
                            (j * (mapTileSize /*+ createSettings.TileWidthPadding*/)) - (mapWidth - 1) * 0.5f,
                            0.0f,
                            (i * (mapTileSize /*+ createSettings.TileDepthPadding*/)) - (mapDepth - 1) * 0.5f),
                        Quaternion.Euler(new Vector3(
                            Random.Range(createSettings.MinRandomRotationJitter.x, createSettings.MaxRandomRotationJitter.x),
                            Random.Range(createSettings.MinRandomRotationJitter.y, createSettings.MaxRandomRotationJitter.y),
                            Random.Range(createSettings.MinRandomRotationJitter.z, createSettings.MaxRandomRotationJitter.z))
                            ), 
                        parentTransform
                        ).GetComponent<MapTile>().setLocation(new MapCoordinate(j,i)));
            }
        }

        // Store map properties
        mapWidthTileCount = createSettings.MapWidthTileCount;
        mapDepthTileCount = createSettings.MapDepthTileCount;

        //Applied the metadata to the maps tiles.
        mapTiles = metaGenerator.apply(mapTiles); 

        instantiated = true;
    }

    public int GetWidthTileCount() { return mapWidthTileCount; }
    public int GetDepthTileCount() { return mapDepthTileCount; }
    public int GetTotalTileCount() { return mapWidthTileCount * mapDepthTileCount; }

    //get the seeded random used for generation.
    public System.Random getMetaSeededRandom() => metaGenerator.random;

    //gets the dictionary containing all the decorations used for tiles.
    public Dictionary<MapTileProperties.TileType,List<GameObject>> getDecor() => metaGenerator.config.map_type_to_decor_list; 
}