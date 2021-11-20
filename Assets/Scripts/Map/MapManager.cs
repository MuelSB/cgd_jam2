using UnityEngine.Assertions;

public static class MapManager
{
    // Class variables
    private static Map map;

    public static void CreateMap(MapCreateSettings createSettings)
    {
        map = new Map(createSettings);
    }

    public static void DestroyMap()
    {
        map.Destroy();
    }

    public static Map GetMap() { return map; }

    public static void GetTileCountTo(MapCoordinate from, MapCoordinate to, out int widthCount, out int depthCount)
    {
        Assert.IsTrue(map.IsValidCoordinate(from) && map.IsValidCoordinate(to) && from != to);

        widthCount = to.x - from.x;
        depthCount = to.y - from.y;
    }
}
