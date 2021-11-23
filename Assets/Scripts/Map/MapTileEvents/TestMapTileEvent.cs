using UnityEngine;

class TestMapTileEvent : MapTileEvent
{
    public override void OnActivated(MapTile tile)
    {
        Debug.Log("Activated test map tile event.");
    }

    public override void OnAdded(MapTile tile)
    {
        Debug.Log("Added test map tile event.");
    }

    public override void OnRemoved(MapTile tile)
    {
        Debug.Log("Removed test map tile event.");
    }
}