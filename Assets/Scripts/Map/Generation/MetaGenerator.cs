using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum MetaDebugHeightMode {
    OFF,
    INTEGRITY_HEIGHT_ON,
    BIOME_STRENGTH_ON
}
public struct MetaGeneratorConfig {
    //general
    public int seed {get; private set;}

    //Integrity Specific
    public int max_tile_integrity {get; private set;}
    public int min_tile_integrity {get; private set;}
    public int tile_x_integrity_frequency {get; private set;}
    public int tile_y_integrity_frequency {get; private set;}

    //Biome Specific
    public MapTileProperties.TileType base_biome {get; private set;}
    public Dictionary<MapTileProperties.TileType, Vector2Int> biome_max_min_strengths {get; private set;}
    public Vector2Int biome_quantity_max_min {get; private set;}
    public MetaDebugHeightMode debug_mode {get; private set;}

    public MetaGeneratorConfig(int _seed, int _max_tile_integrity, int _min_tile_integrity, int _tile_x_integrity_frequency, int _tile_y_integrity_frequency, 
                               MapTileProperties.TileType _base_biome, Dictionary<MapTileProperties.TileType, Vector2Int> _biome_max_min_strengths, Vector2Int _biome_quantity_max_min, MetaDebugHeightMode _debug_mode) {
        seed = _seed;
        max_tile_integrity = _max_tile_integrity;
        min_tile_integrity = _min_tile_integrity;
        tile_x_integrity_frequency = _tile_x_integrity_frequency;
        tile_y_integrity_frequency = _tile_y_integrity_frequency;
        base_biome = _base_biome;
        biome_max_min_strengths = _biome_max_min_strengths;
        biome_quantity_max_min = _biome_quantity_max_min;
        debug_mode = _debug_mode;
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
    public static MapTileProperties.TileType getVillageTypeFromBiomeType(this GameObject _g) => _g.getTileType().getVillageTypeFromBiomeType();
    public static MapTileProperties.TileType getVillageTypeFromBiomeType(this MapTileProperties.TileType _t) => _t switch {
        MapTileProperties.TileType.Forest => MapTileProperties.TileType.Forest_Village,
        MapTileProperties.TileType.Plains => MapTileProperties.TileType.Plains_Village,
        _ => throw new ArgumentException($"The type '{_t}' has no registered village type in getVillageTypeFromBiomeType function!")
    };
    public static Color getColorFromType(this GameObject _g) => _g.getTileType() switch {
        MapTileProperties.TileType.Forest => makeColour(34,139,34),
        MapTileProperties.TileType.Forest_Village => makeColour(0,100,0),
        MapTileProperties.TileType.Plains => makeColour(76,187,23),
        MapTileProperties.TileType.Plains_Village => makeColour(209,226,49), 
        MapTileProperties.TileType.Lake => makeColour(8,146,208),
        MapTileProperties.TileType.Rock => makeColour(83,75,79),
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

    private static int get1DIndex(Vector2Int _2D_index, int _width) => _width*_2D_index.x + _2D_index.y;

    public static List<GameObject> applyStrengthMapToTiles(this Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _grid, List<GameObject> _tiles, int _width) {
        List<GameObject> new_list = _tiles; 

        _grid.Keys.ToList().ForEach(_k => new_list[get1DIndex(_k,_width)] = new_list[get1DIndex(_k,_width)].applyBiome(_grid[_k].Item2));
        return new_list;
    }

    public static List<Vector2Int> getValidNeighbourCoords(this Vector2Int _center, int _max_width, int _max_height) {
        List<Vector2Int> check_points = new List<Vector2Int>() {new Vector2Int(_center.x,_center.y-1), new Vector2Int(_center.x,_center.y+1), new Vector2Int(_center.x-1,_center.y), new Vector2Int(_center.x+1,_center.y)};
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

    private Vector2Int getIndex(int _1D_index) => new Vector2Int(_1D_index/map_reference.GetWidthTileCount(),_1D_index%map_reference.GetDepthTileCount());

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
    private GameObject alterIntegrity(GameObject _go, Vector2Int _pos) => _go.GetComponent<MapTile>().SetProperties(new MapTileProperties().setIntegrity(calculateIntegrityFromCoords(_pos)));

    //inits the biomestrength map. (A way of storing the strength/weight of a tile. This will then later be used to propigate its type to nearby tiles later.)
    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> generateBiomeStrengthMap(List<GameObject> _list, MapTileProperties.TileType _default) => 
        _list.Aggregate(new Dictionary<Vector2Int,(int, MapTileProperties.TileType)>(),(_acc,_b) => {_acc[getIndex(_acc.Count())] = (0,_default);return _acc;});

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

    //for now just adds towns to the heighest biome strength areas that would make sense to have a town.
    private Dictionary<Vector2Int,(int, MapTileProperties.TileType)> applyStructures(Dictionary<Vector2Int,(int, MapTileProperties.TileType)> _biome_map, List<Vector2Int> _starting_points) {
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> new_map = _biome_map;
        _starting_points.Where(_e => new_map[_e].Item2.typeHasVillage()).ToList().ForEach(_t => new_map[_t] = (new_map[_t].Item1,new_map[_t].Item2.getVillageTypeFromBiomeType()));
        return _biome_map;
    }

    //the core of biome generation.
    private List<GameObject> applyBiomes(List<GameObject> _tile_list) {
        List<GameObject> applied_list = _tile_list;
        Dictionary<Vector2Int,(int, MapTileProperties.TileType)> biome_strength_map = generateBiomeStrengthMap(_tile_list,config.base_biome);
        Vector2Int[] sized_array = new Vector2Int[random.Next(config.biome_quantity_max_min.y,config.biome_quantity_max_min.x+1)];
        List<Vector2Int> biome_spawn_location = sized_array.ToList();
        for (int i = 0; i < biome_spawn_location.Count(); i++)
        {
            biome_spawn_location[i] = new Vector2Int(random.Next(0,map_reference.GetWidthTileCount()),random.Next(0,map_reference.GetDepthTileCount()));
        }
        biome_spawn_location.ForEach(_e => {
            MapTileProperties.TileType index = config.biome_max_min_strengths.ElementAt(random.Next(0,config.biome_max_min_strengths.Count())).Key; 
            biome_strength_map = biome_strength_map.setTileTypeAndStrength(_e,index,random.Next(config.biome_max_min_strengths[index].y,config.biome_max_min_strengths[index].x+1));
        });

        biome_strength_map = applyStructures(propagateBiomes(biome_strength_map,biome_spawn_location),biome_spawn_location);

        applied_list = biome_strength_map.applyStrengthMapToTiles(applied_list,map_reference.GetWidthTileCount());

        switch(config.debug_mode) {
            case MetaDebugHeightMode.INTEGRITY_HEIGHT_ON: {
                applied_list.ForEach(_e => _e.transform.position += new Vector3(0,_e.GetComponent<MapTile>().GetProperties().Integrity,0));
                break;
            } case MetaDebugHeightMode.BIOME_STRENGTH_ON: {
                for (int i = 0; i < applied_list.Count(); i++)
                {
                    applied_list[i].transform.position += new Vector3(0,biome_strength_map[getIndex(i)].Item1,0);
                }
                break;
            } default: {
                break;
            }
        }

        // Maybe<List<MapCoordinate>> closest_forest_town = applied_list.getClosestTilesOfType(new MapCoordinate(0,0), MapTileProperties.TileType.Plains_Village);
        // if (closest_forest_town.is_some) {
        //     MapCoordinate town = closest_forest_town.value[0];
        // }

        return applied_list;
    }
}