using System;

[Serializable]
public class MapTileProperties
{
    public enum TileType
    {
        Unassigned,
        Forest,
        Rock, 
        Forest_Village,
        Plains,
        Plains_Village,
        Lake
        // TODO: Decide tile types
    }

    // overrides integrity value and returns self, used in metaGenerator.cs
    public MapTileProperties setIntegrity(float _new_integrity) { Integrity = _new_integrity; return this;} 

    public float Integrity = 1.0f;
    public TileType Type = TileType.Unassigned;
    public bool ContainsEntity = false;

    public Maybe<Entity> tile_enitity = new Maybe<Entity>();

    public bool Alive() { return Integrity <= 0.0f; }

    // TODO: Modular tile event types
}