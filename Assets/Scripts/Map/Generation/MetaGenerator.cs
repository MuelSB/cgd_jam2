using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum MetaDebugHeightMode {
    OFF,
    INTEGRITY_HEIGHT_ON,
    BIOME_STRENGTH_ON,
    INTEGRITY_HEIGHT_WITH_DIVIDER_ON,
}

public enum BiomeTypes {
    PLAINS,
    FOREST,
    WATER,
    ROCKY
}

public struct MetaGeneratorConfig {
    //general
    public int seed {get; private set;}

    //Integrity Specific
    public int max_tile_integrity {get; private set;}
    public int min_tile_integrity {get; private set;}
    public int tile_x_integrity_frequency {get; private set;}
    public int tile_y_integrity_frequency {get; private set;}

    public int tile_integrity_divider {get; private set;} 
    public Vector2Int tile_decrement_range_max_min {get; private set;}

    //Biome Specific
    public MapTileProperties.TileType base_biome {get; private set;}
    public Dictionary<MapTileProperties.TileType, Vector2Int> biome_max_min_strengths {get; private set;}
    public Vector2Int biome_quantity_max_min {get; private set;}
    
    public MetaDebugHeightMode debug_mode {get; private set;}

    //Structure Specific
    public List<MapTileProperties.TileType> included_structures {get; private set;}
    public Vector2Int tower_range_max_min {get; private set;}

    public MetaGeneratorConfig(int _seed, int _max_tile_integrity, int _min_tile_integrity, int _tile_x_integrity_frequency, int _tile_y_integrity_frequency, int _tile_integrity_divider, Vector2Int _tile_decrement_range_max_min,
                               MapTileProperties.TileType _base_biome, Dictionary<MapTileProperties.TileType, Vector2Int> _biome_max_min_strengths, Vector2Int _biome_quantity_max_min, MetaDebugHeightMode _debug_mode, 
                               List<MapTileProperties.TileType> _included_struct, Vector2Int _tower_range_max_min) {
        seed = _seed;
        max_tile_integrity = _max_tile_integrity;
        min_tile_integrity = _min_tile_integrity;
        tile_x_integrity_frequency = _tile_x_integrity_frequency;
        tile_y_integrity_frequency = _tile_y_integrity_frequency;
        tile_integrity_divider = _tile_integrity_divider;
        tile_decrement_range_max_min = _tile_decrement_range_max_min;
        base_biome = _base_biome;
        biome_max_min_strengths = _biome_max_min_strengths;
        biome_quantity_max_min = _biome_quantity_max_min;
        debug_mode = _debug_mode;
        included_structures = _included_struct;
        tower_range_max_min = _tower_range_max_min;
    }
}

//static helper class for generic helper functions.
public static class MetaGeneratorHelper 
{
    //isPlayer: returns if a gameobject with a maptile component has the entity type of player.
    public static bool isPlayer (this GameObject _g) => _g.isOfEntityType(Entity.EntityType.PLAYER);

    //isOfEntityType: returns if a gameobject with a maptile component has the entity type provided.
    public static bool isOfEntityType (this GameObject _g, Entity.EntityType _t ) => _g.GetComponent<MapTile>().GetProperties().tile_enitity.is_some ? _g.GetComponent<MapTile>().GetProperties().tile_enitity.value.type == _t : false;

    //doesEntityOfTypeExistOnGrid: returns if a specified entity type exists somewhere on the map.
    public static bool doesEntityOfTypeExistOnGrid(this List<GameObject> _grid, Entity.EntityType _t) => _grid.Where(_e => _e.GetComponent<MapTile>().GetProperties().tile_enitity.is_some).Where(_x => _x.isOfEntityType(_t)).ToList().Count() > 0;

    //kills a tile at a specific location, returns true if the tile was found at the target position, otherwise returns false.
    public static bool destroyTile(this Map _map, MapCoordinate _target_pos) {
        List<GameObject> valid_tiles = _map.GetTiles().Where(_t => _t.GetComponent<MapTile>().getLocation() == _target_pos).ToList();
        if (valid_tiles.Count > 0) {
            GameObject vt = valid_tiles[0];
            vt.GetComponent<MapTile>().Kill();
            return true;
        } return false;
    }

    //GetBiomeTypes: returns a given tiles biome type. 
    public static BiomeTypes GetBiomeTypes(this GameObject _g) => _g.getTileType().GetBiomeTypes();

    public static BiomeTypes GetBiomeTypes(this MapTileProperties.TileType _tile_type) => _tile_type switch {
        MapTileProperties.TileType.Forest => BiomeTypes.FOREST,
        MapTileProperties.TileType.Rock => BiomeTypes.ROCKY,
        MapTileProperties.TileType.Forest_Village => BiomeTypes.FOREST,
        MapTileProperties.TileType.Plains => BiomeTypes.PLAINS,
        MapTileProperties.TileType.Plains_Village => BiomeTypes.PLAINS,
        MapTileProperties.TileType.Lake => BiomeTypes.WATER,
        MapTileProperties.TileType.Mountain => BiomeTypes.ROCKY,
        MapTileProperties.TileType.Forest_Village_Destroyed => BiomeTypes.FOREST,
        MapTileProperties.TileType.Plains_Village_Destroyed => BiomeTypes.PLAINS,
        MapTileProperties.TileType.Blood_Bog => BiomeTypes.FOREST,
        MapTileProperties.TileType.Lighthouse => BiomeTypes.WATER,
        MapTileProperties.TileType.Travelling_Merchant => BiomeTypes.PLAINS,
        MapTileProperties.TileType.Shrine => BiomeTypes.FOREST,
        MapTileProperties.TileType.Supplies => BiomeTypes.PLAINS,
        MapTileProperties.TileType.Ritual_Circle => BiomeTypes.FOREST,
        _ => throw new ArgumentException($"The type '{_tile_type}' is not registered in GetBiomeTypes function!")
    };
    
    //getPlayerFromGrid: returns the position of the player if it exists, otherwise it returns an empty maybe.
    public static Maybe<MapCoordinate> getPlayerFromGrid(this List<GameObject> _grid) => _grid.doesEntityOfTypeExistOnGrid(Entity.EntityType.PLAYER) ? 
        new Maybe<MapCoordinate>(_grid.Where(_e => _e.GetComponent<MapTile>().GetProperties().tile_enitity.is_some).Where(_x => _x.isPlayer()).ToList()[0].GetComponent<MapTile>().getLocation()) : new Maybe<MapCoordinate>();

    //getClosestTilesOfType: returns a list of positions of the closest position of a specified tile type from a given centre coordinate. 
    //list will have more than one entry if a tiles of the given type are equidistant to the given centre. will return an empty maybe if there is no tile of that type found.
    public static Maybe<List<MapCoordinate>> getClosestTilesOfType(this List<GameObject> _tiles, MapCoordinate _center, MapTileProperties.TileType _type) {
        List<GameObject> targets = _tiles.Where(_e => _e.getTileType() == _type).ToList();
        if (targets.Count < 1) { return new Maybe<List<MapCoordinate>>();}
        int smallest_length = 100000;
        Dictionary<int,List<GameObject>> length_to_gameobjects = new Dictionary<int, List<GameObject>>();
        targets.ForEach(_e => { Vector2Int loc = new Vector2Int((_e.GetComponent<MapTile>().getLocation()-_center).x,(_e.GetComponent<MapTile>().getLocation()-_center).y);
            int current_length = loc.x+loc.y; if (current_length < smallest_length) {smallest_length = current_length;} 
            if (!length_to_gameobjects.Keys.ToList().Contains(current_length)) {length_to_gameobjects[current_length] = new List<GameObject>();} length_to_gameobjects[current_length].Add(_e);
        });
        return new Maybe<List<MapCoordinate>>(targets.Where(_e => {Vector2Int loc = new Vector2Int((_e.GetComponent<MapTile>().getLocation()-_center).x,(_e.GetComponent<MapTile>().getLocation()-_center).y);
            return loc.x+loc.y == smallest_length;
        }).Select(_p => _p.GetComponent<MapTile>().getLocation()).ToList());
    }

    public static MapTileProperties.TileType getTileType(this GameObject _g) => _g.GetComponent<MapTile>().GetProperties().Type;

    //typeIsSpecial: returns true if the given tile is a "special" tile.
    public static bool typeIsSpecial(this GameObject _g) => _g.getTileType().typeIsSpecial();
    public static bool typeIsSpecial(this MapTileProperties.TileType _t) => _t switch {
        MapTileProperties.TileType.Unassigned => false,
        MapTileProperties.TileType.Forest => false,
        MapTileProperties.TileType.Rock => false,
        MapTileProperties.TileType.Forest_Village => true,
        MapTileProperties.TileType.Plains => false,
        MapTileProperties.TileType.Plains_Village => true,
        MapTileProperties.TileType.Lake => false,
        MapTileProperties.TileType.Mountain => true,
        MapTileProperties.TileType.Forest_Village_Destroyed => true,
        MapTileProperties.TileType.Plains_Village_Destroyed => true,
        MapTileProperties.TileType.Tower => true,
        MapTileProperties.TileType.Blood_Bog => true,
        MapTileProperties.TileType.Lighthouse => true,
        MapTileProperties.TileType.Travelling_Merchant => true,
        MapTileProperties.TileType.Shrine => true,
        MapTileProperties.TileType.Supplies => true,
        MapTileProperties.TileType.Ritual_Circle => true,
        _ => throw new ArgumentException($"The type '{_t}' is not registered in typeIsSpecial function!")
    };

    //getClosestSpecialTiles: Returns the closest tile to the center that has a type that counts as special. Can return multiple tiles if multiple special tiles are equadistant to the center.
    //can also return isNone if no tiles of that type are found. Can also be optionally passed a type filter that will only look for tiles that have types that are in the filter. 
    //(Sidenote: if you use a filter, you can search for any tile type, not just special tile types!)
    public static Maybe<List<MapCoordinate>> getClosestSpecialTiles(this List<GameObject> _tiles, MapCoordinate _center) {
        List<MapTileProperties.TileType> filter = new List<MapTileProperties.TileType>();
        Enumerable.Range(0,Enum.GetNames(typeof(MapTileProperties.TileType)).Length).ToList().ForEach(_e => {if (MetaGeneratorHelper.typeIsSpecial((MapTileProperties.TileType)_e)) 
            {filter.Add((MapTileProperties.TileType)_e);}});
        return _tiles.getClosestTiles(_center,filter);
    }
    public static Maybe<List<MapCoordinate>> getClosestTiles(this List<GameObject> _tiles, MapCoordinate _center, List<MapTileProperties.TileType> _filter) {
        List<GameObject> targets = _tiles.Where(_e => _filter.Contains(_e.getTileType())).ToList();
        if (targets.Count < 1) { return new Maybe<List<MapCoordinate>>();}
        int smallest_length = 100000;
        Dictionary<int,List<GameObject>> length_to_gameobjects = new Dictionary<int, List<GameObject>>();
        targets.ForEach(_e => { Vector2Int loc = new Vector2Int((_e.GetComponent<MapTile>().getLocation()-_center).x,(_e.GetComponent<MapTile>().getLocation()-_center).y);
            int current_length = loc.x+loc.y; if (current_length < smallest_length) {smallest_length = current_length;} 
            if (!length_to_gameobjects.Keys.ToList().Contains(current_length)) {length_to_gameobjects[current_length] = new List<GameObject>();} length_to_gameobjects[current_length].Add(_e);
        });
        return new Maybe<List<MapCoordinate>>(targets.Where(_e => {Vector2Int loc = new Vector2Int((_e.GetComponent<MapTile>().getLocation()-_center).x,(_e.GetComponent<MapTile>().getLocation()-_center).y);
            return loc.x+loc.y == smallest_length;
        }).Select(_p => _p.GetComponent<MapTile>().getLocation()).ToList());
    }

    //typeHasVillage: returns if the type of the provided tile has a village variant.
    public static bool typeHasVillage(this GameObject _g) => _g.getTileType().typeHasVillage();
    public static bool typeHasVillage(this MapTileProperties.TileType _t) => _t switch {
        MapTileProperties.TileType.Forest => true,
        MapTileProperties.TileType.Plains => true,
        _ => false
    };

    //isVillageType: returns if the provided type is a valid village.
    public static bool isVillageType(this GameObject _g) => _g.getTileType().isVillageType();
    public static bool isVillageType(this MapTileProperties.TileType _t) => _t switch {
        MapTileProperties.TileType.Forest_Village => true,
        MapTileProperties.TileType.Plains_Village => true,
        _ => false
    };

    //getVillageTypeFromBiomeType: returns the village from a given base tile type if it exists.
    public static MapTileProperties.TileType getVillageTypeFromBiomeType(this GameObject _g) => _g.getTileType().getVillageTypeFromBiomeType();
    public static MapTileProperties.TileType getVillageTypeFromBiomeType(this MapTileProperties.TileType _t) => _t switch {
        MapTileProperties.TileType.Forest => MapTileProperties.TileType.Forest_Village,
        MapTileProperties.TileType.Plains => MapTileProperties.TileType.Plains_Village,
        _ => throw new ArgumentException($"The type '{_t}' has no registered village type in getVillageTypeFromBiomeType function!")
    };
    public static Color getColorFromType(this GameObject _g) => _g.getTileType() switch {
        MapTileProperties.TileType.Forest => makeColour(34,139,34), //green
        MapTileProperties.TileType.Forest_Village => makeColour(0,100,0), //dark green
        MapTileProperties.TileType.Plains => makeColour(76,187,23), //light green
        MapTileProperties.TileType.Plains_Village => makeColour(209,226,49), //olive green (yellow)
        MapTileProperties.TileType.Lake => makeColour(8,146,208), //Blue
        MapTileProperties.TileType.Rock => makeColour(83,75,79), //Grey (pinkish)

        MapTileProperties.TileType.Mountain => makeColour(53,48,51), //Darker grey (pinkish)
        MapTileProperties.TileType.Forest_Village_Destroyed => makeColour(70,99,70), //Grey (+Dark green)
        MapTileProperties.TileType.Plains_Village_Destroyed => makeColour(85,91,20), //Grey (+Olive Green)
        MapTileProperties.TileType.Tower => makeColour(107,30,170), //Purple
        MapTileProperties.TileType.Blood_Bog => makeColour(138,7,7), //Blood red
        MapTileProperties.TileType.Lighthouse => makeColour(127,225,170), //Cyan
        MapTileProperties.TileType.Travelling_Merchant => makeColour(183,104,0), //Orange
        MapTileProperties.TileType.Shrine => makeColour(181,180,173), //Light Grey (like bones :D)
        MapTileProperties.TileType.Supplies => makeColour(234,164,72), //pale orange
        MapTileProperties.TileType.Ritual_Circle => makeColour(191,0,142), //dark pink 

        MapTileProperties.TileType.Unassigned => throw new ArgumentException($"The tile you entered was of type Unassigned! This is not allowed!"),
        _ => throw new ArgumentException($"The type '{_g.getTileType()}' has no registered color in getColorFromType function!")
    };

    public static Color makeColour(int _r, int _g, int _b) => new Color((float)_r/(float)255,(float)_g/(float)255,(float)_b/(float)255);

    public static GameObject applyBiome(this GameObject _g, MapTileProperties.TileType _type) {
        MapTile map_tile = _g.GetComponent<MapTile>();
        MapTileProperties map_properties = map_tile.GetProperties();
        map_properties.Type = _type;
        map_tile.SetProperties(map_properties);
        map_tile.setColour(_g.getColorFromType());
        return _g;
    }

    public static Dictionary<Vector2Int,(int, MapTileProperties.TileType)> setTileTypeAndStrength(this Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _grid, Vector2Int _loc, MapTileProperties.TileType _type, int _strength) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_grid = _grid;
        new_grid[_loc] = (_strength,_type);
        return new_grid;
    }

    public static List<GameObject> applyStrengthMapToTiles(this Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _grid, List<GameObject> _tiles, int _width) {
        List<GameObject> new_list = _tiles; 
        new_list.ForEach(_e => _e.applyBiome(_grid[_e.GetComponent<MapTile>().getLocation().toVector2Int()].Item2));
        // _grid.Keys.ToList().ForEach(_k => new_list[get1DIndex(_k,_width)] = new_list[get1DIndex(_k,_width)].applyBiome(_grid[_k].Item2));
        return new_list;
    }

    public static List<Vector2Int> getValidNeighbourCoords(this Vector2Int _center, int _max_width, int _max_height) {
        List<Vector2Int> check_points = new List<Vector2Int>() {new Vector2Int(_center.x,_center.y-1), new Vector2Int(_center.x,_center.y+1), new Vector2Int(_center.x-1,_center.y), new Vector2Int(_center.x+1,_center.y)};
        return check_points.Where(_e => {if (_center == _e) {return false;} if (_e.x > -1 && _e.x < _max_width) {if (_e.y > -1 && _e.y < _max_height) {return true;}}return false;}).ToList();
    }


    public static List<Vector2Int> getValidMountainNeighbourCoords(this Vector2Int _center, int _max_width, int _max_height) { 
        List<Vector2Int> check_points = new List<Vector2Int>() {
            new Vector2Int(_center.x,_center.y-1), new Vector2Int(_center.x,_center.y+1), new Vector2Int(_center.x-1,_center.y), new Vector2Int(_center.x+1,_center.y),
            new Vector2Int(_center.x-1,_center.y-1), new Vector2Int(_center.x+1,_center.y+1), new Vector2Int(_center.x-1,_center.y+1), new Vector2Int(_center.x+1,_center.y-1),

            new Vector2Int(_center.x-2,_center.y-2), new Vector2Int(_center.x-1,_center.y-2), new Vector2Int(_center.x,_center.y-2), new Vector2Int(_center.x+1,_center.y-2),
            new Vector2Int(_center.x+2,_center.y-2), new Vector2Int(_center.x-2,_center.y-1), new Vector2Int(_center.x+2,_center.y-1), new Vector2Int(_center.x-2,_center.y),
            new Vector2Int(_center.x+2,_center.y), new Vector2Int(_center.x-2,_center.y+1), new Vector2Int(_center.x+2,_center.y+1), new Vector2Int(_center.x-2,_center.y+2),
            new Vector2Int(_center.x-1,_center.y+2), new Vector2Int(_center.x,_center.y+2), new Vector2Int(_center.x+1,_center.y+2), new Vector2Int(_center.x+2,_center.y+2),
        };
        return check_points.Where(_e => {if (_center == _e) {return false;} if (_e.x > -1 && _e.x < _max_width) {if (_e.y > -1 && _e.y < _max_height) {return true;}}return false;}).ToList();
    }
}

//Main Generator Class. Entrypoint can be thought of as the apply function other than the constructor.
public class MetaGenerator
{
    public MetaGeneratorConfig config {get; private set;}
    private Map map_reference;
    private Dictionary<Vector2Int,MapTileProperties> tile_data;

    private System.Random random;
    private int integrity_offset;
    
    //All of this is seeded using the seed in the config, so you can re-make the levels you generate if you save a seed!
    public MetaGenerator(Map _map_reference, MetaGeneratorConfig _config) { 
        map_reference = _map_reference; config = _config; 
        random = new System.Random(config.seed);
        integrity_offset = random.Next(0,999);
    }

    //Main function, applies Integrity to each tile based on perlin noise, then adds biomes and structures afterwards. 
    public List<GameObject> apply(List<GameObject> _tiles) => applyBiomes(_tiles.Aggregate(new List<GameObject>(),(List<GameObject> _acc, GameObject _b) => applyIntegrity(_acc, _b)));

    private Vector2Int getIndex(int _1D_index) => new Vector2Int(_1D_index%map_reference.GetWidthTileCount(),Mathf.FloorToInt(_1D_index/map_reference.GetWidthTileCount()));

    //broken down for ease of debuging/reading. This takes a coord and returns a given integrity value generated from perlin noise. Amplitude is based on the max/min integrity!
    private int calculateIntegrityFromCoords(Vector2Int _coords) {
        int max_variency = config.max_tile_integrity-config.min_tile_integrity;
        float perlin_noise = Mathf.PerlinNoise(((float)((integrity_offset+_coords.x)*config.tile_x_integrity_frequency)/(float)map_reference.GetWidthTileCount()),
                                               ((float)((integrity_offset+_coords.y)*config.tile_y_integrity_frequency)/(float)map_reference.GetDepthTileCount()));
        float variency = max_variency*perlin_noise;
        int floored_varience = (int)Math.Floor(variency);
        return floored_varience+config.min_tile_integrity;
    }

    //an accumulator function used to apply change the integrity of a gameobject and add it back into the accumulated list.
    private List<GameObject> applyIntegrity(List<GameObject> _c, GameObject _e) => _c.Concat(new[] {alterIntegrity(_e,getIndex(_c.Count()))}).ToList();

    //sets the integrity of a gameobject.
    private GameObject alterIntegrity(GameObject _go, Vector2Int _pos) => _go.GetComponent<MapTile>().SetProperties(new MapTileProperties().setIntegrity(calculateIntegrityFromCoords(_pos), 
        config.tile_integrity_divider, config.tile_decrement_range_max_min));

    //inits the biomestrength map. (A way of storing the strength/weight of a tile. This will then later be used to propigate its type to nearby tiles later.)
    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> generateBiomeStrengthMap(int _size, MapTileProperties.TileType _default) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> ret = new Dictionary<Vector2Int, (int, MapTileProperties.TileType)>();
        Enumerable.Range(0,_size).ToList().ForEach(_e => ret[getIndex(_e)] = (0,_default));
        return ret;
    } 

    //this lets the starting positions spread out tiles intil the biomes strength is too low to keep spreading. Clashes with other biomes reduces the strength very quickly.
    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> propagateBiomes(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _biome_map,  List<Vector2Int> _starting_points ) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _biome_map;

        Queue<Vector2Int> active_stack = new Queue<Vector2Int>(_starting_points);
        
        while (active_stack.Count() != 0) {
            Vector2Int current_considered_center = active_stack.Dequeue();
            (int, MapTileProperties.TileType) current_tile_info = new_map[current_considered_center];
            if (current_tile_info.Item1 == 1) {continue;}
            List<Vector2Int> valid_neighbourhood = current_considered_center.getValidNeighbourCoords(map_reference.GetWidthTileCount(),map_reference.GetDepthTileCount()).Where(_n => !active_stack.Contains(_n)).ToList();
            foreach (Vector2Int position in valid_neighbourhood)
            {
                if (!new_map.ContainsKey(position)) {
                    throw new Exception($"Something is wrong! Position {position.x}:{position.y} is not on the biome grid!");
                }
                (int, MapTileProperties.TileType) possible_tile = new_map[position];
                if (possible_tile.Item2 != current_tile_info.Item2) {
                    int strength = possible_tile.Item1 - (current_tile_info.Item1-1);
                    if (strength > 0) {
                        new_map[position] = (strength,new_map[position].Item2);
                    } else {
                        new_map[position] = ((current_tile_info.Item1-1)-possible_tile.Item1,current_tile_info.Item2);
                        active_stack.Enqueue(position);
                    }
                }
            }
        }
        return new_map;
    }

    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddVillages(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map, List<Vector2Int> _starting_points, List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
        //Add villages to biome spawn locations, if villages are allowed on the tile and that tiles village is allowed to spawn.
        _starting_points.Where(_e => new_map[_e].Item2.typeHasVillage()).ToList().ForEach
            (_t => new_map[_t] = (new_map[_t].Item1, _allowed_structures.Contains(new_map[_t].Item2.getVillageTypeFromBiomeType()) ? new_map[_t].Item2.getVillageTypeFromBiomeType() : new_map[_t].Item2 ));
        return new_map;
    }

    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddMountains(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map,List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
        //Add mountains to rock tiles if the surrounding tiles are also rock or mountain if allowed to spawn. Moore Neighbour with R=2
        if (_allowed_structures.Contains(MapTileProperties.TileType.Mountain)) {
            new_map.Keys.ToList().ForEach(_k => {
                MapTileProperties.TileType current_type = new_map[_k].Item2;
                if (current_type == MapTileProperties.TileType.Rock) {
                    List<Vector2Int> vn = _k.getValidMountainNeighbourCoords(map_reference.GetWidthTileCount(),map_reference.GetDepthTileCount());
                    List<MapTileProperties.TileType> vnt = vn.Aggregate(new List<MapTileProperties.TileType>(),(_a, _b) => _a.Concat(new[] {new_map[_b].Item2}).ToList());
                    int vnt_length = vnt.Count;
                    if (vnt.Where(_e => _e == MapTileProperties.TileType.Rock || _e == MapTileProperties.TileType.Mountain).ToList().Count == vnt_length) {
                        new_map[_k] = (new_map[_k].Item1,MapTileProperties.TileType.Mountain);
                    }
                }
            });
        }
        return new_map;
    }

    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddTowers(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map,List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
         //Add towers randomly in forests or rock biomes if allowed to spawn.
        if (_allowed_structures.Contains(MapTileProperties.TileType.Tower)) {
            List<KeyValuePair<Vector2Int,(int,MapTileProperties.TileType)>> valid_spawn_tiles = new_map.Where(_e => _e.Value.Item2 == MapTileProperties.TileType.Forest || _e.Value.Item2 == MapTileProperties.TileType.Rock).ToList();
            int tower_quant = random.Next(config.tower_range_max_min.y, config.tower_range_max_min.x+1);
            if (tower_quant > valid_spawn_tiles.Count) { tower_quant = valid_spawn_tiles.Count;}
            if (tower_quant > 0) {
                List<KeyValuePair<Vector2Int,(int,MapTileProperties.TileType)>> spawn_points = new List<KeyValuePair<Vector2Int, (int, MapTileProperties.TileType)>>();
                while (spawn_points.Count != tower_quant) {
                    KeyValuePair<Vector2Int,(int,MapTileProperties.TileType)> selected = valid_spawn_tiles[random.Next(0,valid_spawn_tiles.Count)];
                    if (!spawn_points.Contains(selected)) {
                        spawn_points.Add(selected);
                    }
                }
                spawn_points.ForEach(_kvp => new_map[_kvp.Key] = (_kvp.Value.Item1,MapTileProperties.TileType.Tower));
            }
        }
        return new_map;
    }

    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddBloodBog(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map,List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
        //Will spawn one blood bog on water tiles cardinally adjacent to a forest if allowed to spawn.
        if (_allowed_structures.Contains(MapTileProperties.TileType.Blood_Bog)) {
            List<(Vector2Int,int)> valid_spawn_points = new List<(Vector2Int, int)>();
            new_map.Keys.ToList().ForEach(_k => {
                if (new_map[_k].Item2 == MapTileProperties.TileType.Lake) {
                    List<Vector2Int> vn = _k.getValidNeighbourCoords(map_reference.GetWidthTileCount(),map_reference.GetDepthTileCount());
                    List<MapTileProperties.TileType> vnt = vn.Aggregate(new List<MapTileProperties.TileType>(),(_a,_b) => _a.Concat(new[]{new_map[_b].Item2}).ToList());
                    if (vnt.Contains(MapTileProperties.TileType.Forest)) {
                        valid_spawn_points.Add((_k, new_map[_k].Item1));
                    }
                }
            });
            if (valid_spawn_points.Count > 0) {
                (Vector2Int,int) selected = valid_spawn_points[random.Next(0,valid_spawn_points.Count)];
                new_map[selected.Item1] = (selected.Item2,MapTileProperties.TileType.Blood_Bog);
            }
        }
        return new_map;
    }

    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddLighthouse(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map,List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
        //Will spawn lighthouse on a tile cardinally adjacent to a lake if allowed to spawn.
        if (_allowed_structures.Contains(MapTileProperties.TileType.Lighthouse)) {
            List<(Vector2Int,int)> valid_spawn_points = new List<(Vector2Int, int)>();
            new_map.Keys.ToList().ForEach(_k => {
                if (new_map[_k].Item2 != MapTileProperties.TileType.Lake) {
                    List<Vector2Int> vn = _k.getValidNeighbourCoords(map_reference.GetWidthTileCount(),map_reference.GetDepthTileCount());
                    List<MapTileProperties.TileType> vnt = vn.Aggregate(new List<MapTileProperties.TileType>(),(_a,_b) => _a.Concat(new[]{new_map[_b].Item2}).ToList());
                    if (vnt.Contains(MapTileProperties.TileType.Lake)) {
                        valid_spawn_points.Add((_k, new_map[_k].Item1));
                    }
                }
            });
            if (valid_spawn_points.Count > 0) {
                (Vector2Int,int) selected = valid_spawn_points[random.Next(0,valid_spawn_points.Count)];
                new_map[selected.Item1] = (selected.Item2,MapTileProperties.TileType.Lighthouse);
            }
        }
        return new_map;
    }

    
    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddTravelingMerchent(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map,List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
        //Has a 1 in 100 chance to spawn a traveling merchent tile on a plain if allowed to spawn.
        if (_allowed_structures.Contains(MapTileProperties.TileType.Travelling_Merchant)) {
            List<(Vector2Int,int)> valid_spawn_points = new List<(Vector2Int, int)>();
            new_map.Keys.ToList().ForEach(_k => {
                if (new_map[_k].Item2 == MapTileProperties.TileType.Plains) {
                    valid_spawn_points.Add((_k, new_map[_k].Item1));
                }
            });
            if (valid_spawn_points.Count > 0) {
                valid_spawn_points.ForEach(_p => {
                    int spawn_val = random.Next(0,101);
                    if (spawn_val == 1) {
                        new_map[_p.Item1] = (_p.Item2,MapTileProperties.TileType.Travelling_Merchant);
                    }
                });
            }
        }
        return new_map;
    }

    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddShrine(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map,List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
        //Has a 1 in 100 chance to spawn a shrine tile on a forest if allowed to spawn.
        if (_allowed_structures.Contains(MapTileProperties.TileType.Shrine)) {
            List<(Vector2Int,int)> valid_spawn_points = new List<(Vector2Int, int)>();
            new_map.Keys.ToList().ForEach(_k => {
                if (new_map[_k].Item2 == MapTileProperties.TileType.Forest) {
                    valid_spawn_points.Add((_k, new_map[_k].Item1));
                }
            });
            if (valid_spawn_points.Count > 0) {
                valid_spawn_points.ForEach(_p => {
                    int spawn_val = random.Next(0,101);
                    if (spawn_val == 1) {
                        new_map[_p.Item1] = (_p.Item2,MapTileProperties.TileType.Shrine);
                    }
                });
            }
        }
        return new_map;
    }

        private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> AddSupplies(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _new_map,List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _new_map;
        //Has a 1 in 80 chance to spawn a supplies tile on a plain if allowed to spawn.
        if (_allowed_structures.Contains(MapTileProperties.TileType.Supplies)) {
            List<(Vector2Int,int)> valid_spawn_points = new List<(Vector2Int, int)>();
            new_map.Keys.ToList().ForEach(_k => {
                if (new_map[_k].Item2 == MapTileProperties.TileType.Plains) {
                    valid_spawn_points.Add((_k, new_map[_k].Item1));
                }
            });
            if (valid_spawn_points.Count > 0) {
                valid_spawn_points.ForEach(_p => {
                    int spawn_val = random.Next(0,81);
                    if (spawn_val == 1) {
                        new_map[_p.Item1] = (_p.Item2,MapTileProperties.TileType.Supplies);
                    }
                });
            }
        }
        return new_map;
    }
    
    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> applyStructures(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _biome_map, List<Vector2Int> _starting_points, List<MapTileProperties.TileType> _allowed_structures) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _biome_map;

        new_map = AddSupplies(
                  AddShrine(
                  AddTravelingMerchent(
                  AddLighthouse(
                  AddBloodBog(
                  AddTowers(
                  AddMountains(
                  AddVillages(
                  new_map,_starting_points,_allowed_structures),_allowed_structures),_allowed_structures),_allowed_structures),_allowed_structures),_allowed_structures),_allowed_structures),_allowed_structures);
        return new_map;
    }

    //the core of biome generation.
    private List<GameObject> applyBiomes(List<GameObject> _tile_list) {
        List<GameObject> applied_list = _tile_list;
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> biome_strength_map = generateBiomeStrengthMap(_tile_list.Count(),config.base_biome);
        Vector2Int[] sized_array = new Vector2Int[random.Next(config.biome_quantity_max_min.y,config.biome_quantity_max_min.x+1)];
        List<Vector2Int> biome_spawn_location = new List<Vector2Int>();
        for (int i = 0; i < sized_array.Count(); i++)
        {
            if (biome_spawn_location.Count < applied_list.Count) {
                bool locating_spawn_point = true;
                Vector2Int spawn_point;
                while (locating_spawn_point) {
                    spawn_point = getIndex(random.Next(0,applied_list.Count));
                    if (!biome_spawn_location.Contains(spawn_point)) {
                        locating_spawn_point = false;
                        biome_spawn_location.Add(spawn_point);
                    }
                }
            }
        }
        biome_spawn_location.ForEach(_e => {
            MapTileProperties.TileType index = config.biome_max_min_strengths.ElementAt(random.Next(0,config.biome_max_min_strengths.Count())).Key; 
            biome_strength_map = biome_strength_map.setTileTypeAndStrength(_e,index,random.Next(config.biome_max_min_strengths[index].y,config.biome_max_min_strengths[index].x+1));
        });

        biome_strength_map = applyStructures(propagateBiomes(biome_strength_map,biome_spawn_location),biome_spawn_location,config.included_structures);

        applied_list = biome_strength_map.applyStrengthMapToTiles(applied_list,map_reference.GetWidthTileCount());

        switch(config.debug_mode) {
            case MetaDebugHeightMode.INTEGRITY_HEIGHT_WITH_DIVIDER_ON: {
                applied_list.ForEach(_e => _e.transform.position += new Vector3(0,_e.GetComponent<MapTile>().GetProperties().getHeightFromIntegrity(),0));
                break;
            }
            case MetaDebugHeightMode.INTEGRITY_HEIGHT_ON: {
                applied_list.ForEach(_e => _e.transform.position += new Vector3(0,_e.GetComponent<MapTile>().GetProperties().Integrity/2,0));
                break;
            } case MetaDebugHeightMode.BIOME_STRENGTH_ON: {
                applied_list.ForEach(_e => _e.transform.position += new Vector3(0,biome_strength_map[_e.GetComponent<MapTile>().getLocation().toVector2Int()].Item1,0));
                break;
            } default: {
                break;
            }
        }

        // Maybe<List<MapCoordinate>> closest_forest_town = applied_list.getClosestSpecialTiles(new MapCoordinate(0,0));
        // if (closest_forest_town.is_some) {
        //     MapCoordinate town = closest_forest_town.value[0];
        // }

        return applied_list;
    }
}