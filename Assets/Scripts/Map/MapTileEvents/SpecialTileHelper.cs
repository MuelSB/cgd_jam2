using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SpecialTileHelper
{
    //DestroySpecialTile: takes any special tile and transforms it into its destroyed version. Does NOT actually destroy the tile.
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
            _ => throw new System.ArgumentException($"tile type {tile_type} was not registered in MakeNormalTile switch expression!")
        };
        return _special_tile.transformTile(new_type,_decor,_random);
    }

    //turns a tile from one type to another (including colour, decor, etc.)
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

    //spawnHostileInClosestEmptySpace: Will spawn a specific enemy type in the nearest empty spot from a given centre. 
    public static void spawnHostileInClosestEmptySpace(this GameObject _center, EnemiesData.EnemyType _type, List<GameObject> _tile_map, EnemyManager _enemy_manager) {
        Maybe<List<MapCoordinate>> spawn_location = _tile_map.findClosestSpawnPoint(_center.GetComponent<MapTile>().getLocation());
        if (spawn_location.is_some) {
            _enemy_manager.CreateEnemyOfType(spawn_location.value[0],EnemiesData.Instance.enemiesData.Where(_e => _e.type == _type).ToList()[0]);
        } 
    }

    //doSpecialTileAction: Executes the special tiles functionality and optionally removes the tiles special state by calling DestroySpecialTile.
    public static void doSpecialTileAction(this GameObject _tile, Map _map, EnemyManager _enemy_manager, bool _destroy_special_statue_after = false) {
        if (!_tile.typeIsSpecial()) { throw new System.ArgumentException("Tile given to doSpecialTileAction was not special!");}
        
        Player player;
        if (_map.GetTiles().getPlayerFromGrid().is_some) {
            player = (Player)_map.GetTile(_map.GetTiles().getPlayerFromGrid().value).GetProperties().tile_enitity.value;
        } else { throw new System.ArgumentException("Player does not exist on the provided map? Thats not good."); }

        Enemy boss;
        if (_map.GetTiles().getBossFromGrid().is_some) {
            boss = (Enemy)_map.GetTile(_map.GetTiles().getBossFromGrid().value).GetProperties().tile_enitity.value;
        } else { throw new System.ArgumentException("Boss does not exist on the provided map? Thats not good."); }

        List<GameObject> tiles = _map.GetTiles();

        System.Random random = _map.getMetaSeededRandom();

        switch(_tile.getTileType()) {
            //villages heal!
            case MapTileProperties.TileType.Forest_Village:
            case MapTileProperties.TileType.Plains_Village: { doVillageHeal(player,random); break; }
            //Mountains are impassible, but this should probably be handled elsewhere.
            case MapTileProperties.TileType.Mountain: { break; }
            //Destroyed villages either find an item or spawn a monster.
            case MapTileProperties.TileType.Forest_Village_Destroyed: 
            case MapTileProperties.TileType.Plains_Village_Destroyed: { doDestroyedVillageItemOrMonster(_tile,player,tiles,_enemy_manager,random); break; }
            //Towers give players a spell
            case MapTileProperties.TileType.Tower: { doTowerSpell(player,random); break; }
            //Blood bog Spawns hard monster that gives a bunch of xp?
            case MapTileProperties.TileType.Blood_Bog: { doBloodBogBoss(_tile,tiles,_enemy_manager,random); break; }
            //Lighthouse Stuns the boss
            case MapTileProperties.TileType.Lighthouse: { doLighthouseStun(boss); break; }
            //Travelling Merchant Gives an item.
            case MapTileProperties.TileType.Travelling_Merchant: { doMerchantGiveItem(player,random); break; }
            //Shrine Spawns a bunch of weak hostiles.
            case MapTileProperties.TileType.Shrine: { doShrineAmbush(_tile,tiles,_enemy_manager,random); break; }
            //Supplies give xp
            case MapTileProperties.TileType.Supplies: { doSuppliesXPIncrease(player,random); break; }
            //Ritual Circles does something random
            case MapTileProperties.TileType.Ritual_Circle: { doRitualShananagins(_tile,player,boss,tiles,_enemy_manager,random); break; }
            default: { throw new System.ArgumentException($"tile type {_tile.getTileType()} was not registered in doSpecialTileAction switch!"); }
        }
        if (_destroy_special_statue_after) {
            _tile.DestroySpecialTile(_map.getDecor(),_map.getMetaSeededRandom());
        }
    }

    private static void doVillageHeal(Player _player, System.Random _random) {
        _player.Heal(_random.Next(2,5));
    }

    private static void doDestroyedVillageItemOrMonster(GameObject _tile, Player _player, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) {
        if (_random.Next(0,10) > 4) {
            doVillageHeal(_player,_random);
        } else {
            int rat_size = _random.Next(1,4);
            for (int i = 0; i < rat_size; i++)
            {
                _tile.spawnHostileInClosestEmptySpace(EnemiesData.EnemyType.RAT,_tiles,_enemy_manager);
            }
        }
    }

    private static void doTowerSpell(Player _player, System.Random _random) {
        throw new System.NotImplementedException("doTowerSpell in SpecilaTileHelper");
    }

    private static void doBloodBogBoss(GameObject _tile, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) {
        //_tile.spawnHostileInClosestEmptySpace() waiting on boss to exist
        throw new System.NotImplementedException("doBloodBogBoss in SpecilaTileHelper");
    }

    private static void doLighthouseStun(Enemy _boss) {
        throw new System.NotImplementedException("doLighthouseStun in SpecilaTileHelper");
    }

    private static void doMerchantGiveItem(Player _player, System.Random _random) {
        throw new System.NotImplementedException("doMerchantGiveItem in SpecilaTileHelper");
    }

    private static void doShrineAmbush(GameObject _tile, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) { //bats
        int bat_size = _random.Next(4,9);
        for (int i = 0; i < bat_size; i++)
        {
            _tile.spawnHostileInClosestEmptySpace(EnemiesData.EnemyType.BAT,_tiles,_enemy_manager);
        }
    }

    private static void doSuppliesXPIncrease(Player _player, System.Random _random) {
        throw new System.NotImplementedException("doSuppliesXPIncrease in SpecilaTileHelper");
    }

    private static void doRitualShananagins(GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) {
        Dictionary<int,Action<GameObject, Player, Enemy, List<GameObject>, EnemyManager, System.Random>> chaos_cauldron = 
        new Dictionary<int, Action<GameObject, Player,  Enemy, List<GameObject>, EnemyManager, System.Random>>() {
            {0, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doVillageHeal(_player,_random);}},
            {1, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doDestroyedVillageItemOrMonster(_tile,_player,_tiles,_enemy_manager,_random);}},
            {2, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doTowerSpell(_player,_random);}},
            {3, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doBloodBogBoss(_tile,_tiles,_enemy_manager,_random);}},
            {4, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doLighthouseStun(_boss);}},
            {5, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doMerchantGiveItem(_player,_random);}},
            {6, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doShrineAmbush(_tile,_tiles,_enemy_manager,_random);}},
            {7, (GameObject _tile, Player _player, Enemy _boss, List<GameObject> _tiles, EnemyManager _enemy_manager, System.Random _random) => {doSuppliesXPIncrease(_player,_random);}}};
        int stir = _random.Next(0,8);
        chaos_cauldron[stir](_tile,_player,_boss,_tiles,_enemy_manager,_random);
    }
}