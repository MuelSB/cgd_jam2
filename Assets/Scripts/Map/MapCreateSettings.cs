using System;
using UnityEngine;

[Serializable]
public struct MapCreateSettings
{
    public GameObject MapTilePrefabReference;
    public int MapWidthTileCount;
    public int MapDepthTileCount;
    public Vector3 MinRandomRotationJitter;
    public Vector3 MaxRandomRotationJitter;
    //public float TileWidthPadding;
    //public float TileDepthPadding;
}