using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpecialTileHelper
{
    public static GameObject DestroySpecialTile(this GameObject _special_tile, Dictionary<MapTileProperties.TileType,List<GameObject>> _decor, System.Random _random) {
        if (!_special_tile.typeIsSpecial()) { throw new System.ArgumentException("Tile given to MakeNormalTile was not special!");}
        MapTileProperties.TileType tile_type = _special_tile.getTileType();
        MapTileProperties.TileType new_type = tile_type switch {
            MapTileProperties.TileType.Forest_Village => MapTileProperties.TileType.Forest_Village_Destroyed,
            MapTileProperties.TileType.Plains_Village => MapTileProperties.TileType.Plains_Village_Destroyed,
            MapTileProperties.TileType.Mountain => MapTileProperties.TileType.Rock,
            MapTileProperties.TileType.Forest_Village_Destroyed => MapTileProperties.TileType.Forest,
            MapTileProperties.TileType.Plains_Village_Destroyed => MapTileProperties.TileType.Plains,
            MapTileProperties.TileType.Tower => MapTileProperties.TileType.Rock,
            MapTileProperties.TileType.Blood_Bog => MapTileProperties.TileType.Forest,
            MapTileProperties.TileType.Lighthouse => MapTileProperties.TileType.Lake,
            MapTileProperties.TileType.Travelling_Merchant => MapTileProperties.TileType.Plains,
            MapTileProperties.TileType.Shrine => MapTileProperties.TileType.Forest,
            MapTileProperties.TileType.Supplies => MapTileProperties.TileType.Plains,
            MapTileProperties.TileType.Ritual_Circle => MapTileProperties.TileType.Plains,
            _ => throw new System.ArgumentException($"tile type {tile_type} was not registered in MakeNormalTile switch!")
        };
        return _special_tile.transformTile(new_type,_decor,_random);
    }

    public static GameObject transformTile(this GameObject _tile_to_chance, MapTileProperties.TileType _new_type, Dictionary<MapTileProperties.TileType,List<GameObject>> _decor, System.Random _random) {
        MapTile map_tile = _tile_to_chance.GetComponent<MapTile>();
        MapTileProperties new_properties = map_tile.GetProperties();
        new_properties.Type = _new_type;
        map_tile.SetProperties(new_properties);
        map_tile.setColour(_tile_to_chance.getColorFromType());
        map_tile.removeDecor();
        if (_decor.ContainsKey(_new_type)) {
            map_tile.spawnDecor(_decor[_new_type][_random.Next(0,_decor[_new_type].Count)]);
        }
        return _tile_to_chance;
    }
}
