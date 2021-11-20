using System;

[Serializable]
public struct MapCoordinate
{
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
        return lhs.x == rhs.x && lhs.x == rhs.x;
    }

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

    public int x;
    public int y;
}