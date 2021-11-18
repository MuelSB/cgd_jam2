using System;

[Serializable]
public struct MapCoordinate
{
    public MapCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;
}