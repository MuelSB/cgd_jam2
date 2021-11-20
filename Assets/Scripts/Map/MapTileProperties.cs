using System;

[Serializable]
public class MapTileProperties
{
    public enum TileType
    {
        Unassigned,
        Forest,
        Rock, 
        Village,
        // TODO: Decide tile types
    }

    public float Integrity = 1.0f;
    public TileType Type = TileType.Unassigned;
    public bool ContainsEntity = false;

    public bool Alive() { return Integrity <= 0.0f; }

    // TODO: Modular tile event types
}