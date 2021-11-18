using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mapTilePrefabReference;

    private void Start()
    {
        // Create the game map
        MapCreateSettings mapCreateSettings = new MapCreateSettings();
        mapCreateSettings.MapTilePrefabReference = mapTilePrefabReference;
        mapCreateSettings.MapWidthTileCount = 3;
        mapCreateSettings.MapDepthTileCount = 3;
        //mapCreateSettings.TileWidthPadding = 0;
        //mapCreateSettings.TileDepthPadding = 0;

        MapManager.CreateMap(mapCreateSettings);
    }
}
