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
}
