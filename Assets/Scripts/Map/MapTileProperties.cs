using System;

[Serializable]
public class MapTileProperties
{
    public float Integrity = 1.0f;

    public bool Alive() { return Integrity <= 0.0f; }

    // TODO: Contains entity
    // TODO: Tile type menu
    // TODO: Tile neighbours
    // TODO: Modular tile event types
}