using UnityEngine;

public class MapTile : MonoBehaviour
{
    // Class properties
    private MapTileProperties properties;

    public void SetProperties(MapTileProperties properties) { this.properties = properties; }
    public MapTileProperties GetProperties() { return properties; }
}
