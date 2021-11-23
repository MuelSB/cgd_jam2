public abstract class MapTileEvent
{
    // Called when a tile event is activated
    public abstract void OnActivated(MapTile tile);

    // Called when a tile event is added to a tile
    public abstract void OnAdded(MapTile tile);

    // Called when a tile event is removed from a tile
    public abstract void OnRemoved(MapTile tile);
}