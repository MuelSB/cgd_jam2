using System;
using UnityEngine;

[Serializable]
public struct MapCreateSettings
{
    public GameObject MapTilePrefabReference;
    public int MapWidthTileCount;
    public int MapDepthTileCount;
    //public float TileWidthPadding;
    //public float TileDepthPadding;
}