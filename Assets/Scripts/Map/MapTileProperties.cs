using System;
using System.Collections.Generic;
using UnityEngine;

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
        Lake,
        Mountain,
        Forest_Village_Destroyed,
        Plains_Village_Destroyed,
        Tower,
        Blood_Bog,
        Lighthouse,
        Travelling_Merchant,
        Shrine,
        Supplies,
        Ritual_Circle

        // TODO: Decide tile types
    }

    // overrides integrity value and returns self, used in metaGenerator.cs
    public MapTileProperties setIntegrity(float _new_integrity) { Integrity = _new_integrity; return this;} 

    public MapTile tile;
    public float Integrity = 1.0f;
    public TileType Type = TileType.Unassigned;
    public bool ContainsEntity = false;

    public Maybe<Entity> tile_enitity = new Maybe<Entity>();

    private Dictionary<ulong, MapTileEvent> events = new Dictionary<ulong, MapTileEvent>();

    private ulong availableEventID = 0;

    public bool Alive() { return Integrity <= 0.0f; }

    public ulong AddEvent(MapTileEvent ev)
    {
        ev.OnAdded(tile);

        var id = availableEventID;
        events.Add(id, ev);

        ++availableEventID;

        return id;
    }

    public void ActivateEvents()
    {
        foreach(KeyValuePair<ulong, MapTileEvent> ev in events)
        {
            ev.Value.OnActivated(tile);
        }
    }

    public void RemoveEvent(ulong id)
    {
        events[id].OnRemoved(tile);
        events.Remove(id);
    }

    public void ClearEvents()
    {
        foreach (KeyValuePair<ulong, MapTileEvent> ev in events)
        {
            ev.Value.OnRemoved(tile);
        }

        events.Clear();
        availableEventID = 0;
    }
}