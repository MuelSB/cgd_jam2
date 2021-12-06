using UnityEngine.Assertions;
using UnityEngine;

public static class MapManager
{
    // Class variables
    private static Map map;

    public static void CreateMap(MapCreateSettings createSettings, MetaGeneratorConfig metaGeneratorConfig, Transform parentTransform)
    {
        map = new Map(createSettings, metaGeneratorConfig, parentTransform);
    }

    public static void DestroyMap()
    {
        map.Destroy();
    }

    public static Map GetMap() { return map; }

    public static void GetTileCountTo(MapCoordinate from, MapCoordinate to, out int widthCount, out int depthCount)
    {
        Assert.IsTrue(map.IsValidCoordinate(from) && map.IsValidCoordinate(to));

        widthCount = to.x - from.x;
        depthCount = to.y - from.y;
    }

    public static void GetAbsoluteTileCountTo(MapCoordinate from, MapCoordinate to, out int widthCount, out int depthCount)
    {
        GetTileCountTo(from, to, out widthCount, out depthCount);
        widthCount = Mathf.Abs(widthCount);
        depthCount = Mathf.Abs(depthCount);
    }

    // This function is not correct. It treats map coordinate 0, 0 as the tile at the center of the map
    // Map coordinate 0, 0 should be the bottom left tile
    public static Maybe<MapCoordinate> WorldSpaceToMapCoordinate(Vector3 worldSpacePosition)
    {
        var tileSize = map.GetTileSize();
        var widthTileCount = map.GetWidthTileCount();
        var depthTileCount = map.GetDepthTileCount();

        // Is the point in the graph
        if (worldSpacePosition.x >= -((widthTileCount * tileSize) / 2) &&
            worldSpacePosition.x <= ((widthTileCount * tileSize) / 2) &&
            worldSpacePosition.z >= -((depthTileCount * tileSize) / 2) &&
            worldSpacePosition.z <= ((depthTileCount * tileSize) / 2))
        {
            return new Maybe<MapCoordinate>(new MapCoordinate((int)Mathf.Floor(worldSpacePosition.x / tileSize), (int)Mathf.Floor(worldSpacePosition.z / tileSize)));
        }

        return new Maybe<MapCoordinate>();
    }
}
