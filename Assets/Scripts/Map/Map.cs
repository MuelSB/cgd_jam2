using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Map
{
    // Class variables
    private int mapWidthTileCount;
    private int mapDepthTileCount;
    private List<GameObject> mapTiles = new List<GameObject>();
    private bool instantiated = false;

    public Map(MapCreateSettings createSettings)
    {
        Destroy();
        Instantiate(createSettings);
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

    public GameObject GetTileObject(MapCoordinate mapCoordinate)
    {
        Assert.IsTrue(mapCoordinate.x >= 0 && mapCoordinate.y >= 0 && mapCoordinate.x < mapWidthTileCount && mapCoordinate.y < mapDepthTileCount,
            "Attempting to access map with invalid grid coordinate.");

        return mapTiles[mapCoordinate.y * mapDepthTileCount + mapCoordinate.x];
    }

    public MapTileProperties GetTileProperties(MapCoordinate mapCoordinate)
    {
        return GetTile(mapCoordinate).GetProperties();
    }

    public MapTile GetTile(MapCoordinate mapCoordinate)
    {
        return GetTileObject(mapCoordinate).GetComponent<MapTile>();
    }

    private void Instantiate(MapCreateSettings createSettings)
    {
        Assert.IsFalse(instantiated, "Attempting to instantiate the map without destroying existing map data.");

        // Compute tile properties
        var tileWidth = createSettings.MapTilePrefabReference.transform.localScale.x;
        var tileDepth = createSettings.MapTilePrefabReference.transform.localScale.z;

        var mapWidth = tileWidth * createSettings.MapWidthTileCount;
        var mapDepth = tileDepth * createSettings.MapDepthTileCount;

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
                            (j * (tileWidth /*+ createSettings.TileWidthPadding*/)) - (mapWidth - 1) * 0.5f,
                            0.0f,
                            (i * (tileDepth /*+ createSettings.TileDepthPadding*/)) - (mapDepth - 1) * 0.5f),
                        Quaternion.Euler(new Vector3(
                            Random.Range(createSettings.MinRandomRotationJitter.x, createSettings.MaxRandomRotationJitter.x),
                            Random.Range(createSettings.MinRandomRotationJitter.y, createSettings.MaxRandomRotationJitter.y),
                            Random.Range(createSettings.MinRandomRotationJitter.z, createSettings.MaxRandomRotationJitter.z))
                            )
                        ));
            }
        }

        // Store map properties
        mapWidthTileCount = createSettings.MapWidthTileCount;
        mapDepthTileCount = createSettings.MapDepthTileCount;

        instantiated = true;
    }

    public int GetWidthTileCount() { return mapWidthTileCount; }
    public int GetDepthTileCount() { return mapDepthTileCount; }
    public int GetTotalTileCount() { return mapWidthTileCount * mapDepthTileCount; }
}