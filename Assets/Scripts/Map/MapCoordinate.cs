using System;
using UnityEngine;

[Serializable]
public class MapCoordinate
{
    public MapCoordinate(Vector2Int _new_vec) {
        this.x = _new_vec.x;
        this.y = _new_vec.y;
    }
    public MapCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator !=(MapCoordinate lhs, MapCoordinate rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }

    public static bool operator ==(MapCoordinate lhs, MapCoordinate rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static MapCoordinate operator -(MapCoordinate lhs, MapCoordinate rhs) => new MapCoordinate(lhs.x-rhs.x,lhs.y-rhs.y);

    public override string ToString()
    {
        return "[X: " + x.ToString() + ", " + "Y: " + y.ToString() + "]";
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public Vector2Int toVector2Int() => new Vector2Int(x,y);

    public int x;
    public int y;
}