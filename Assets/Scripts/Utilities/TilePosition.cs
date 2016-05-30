using UnityEngine;


public class TilePosition
{
    public static Vector2 NORTH { get { return new Vector2( 0,  1); } }
    public static Vector2 EAST  { get { return new Vector2( 1,  0); } }
    public static Vector2 SOUTH { get { return new Vector2( 0, -1); } }
    public static Vector2 WEST  { get { return new Vector2(-1,  0); } }


    public static Vector2 North(int X, int Y)
    {
        return new Vector2(X, Y) + NORTH;
    }

    public static Vector2 East(int X, int Y)
    {
        return new Vector2(X, Y) + EAST;
    }

    public static Vector2 South(int X, int Y)
    {
        return new Vector2(X, Y) + SOUTH;
    }

    public static Vector2 West(int X, int Y)
    {
        return new Vector2(X, Y) + WEST;
    }

    public static Vector2 NorthEast(int X, int Y)
    {
        return new Vector2(X, Y) + NORTH + EAST;
    }

    public static Vector2 SouthEast(int X, int Y)
    {
        return new Vector2(X, Y) + SOUTH + EAST;
    }

    public static Vector2 SouthWest(int X, int Y)
    {
        return new Vector2(X, Y) + SOUTH + WEST;
    }

    public static Vector2 NorthWest(int X, int Y)
    {
        return new Vector2(X, Y) + NORTH + WEST;
    }
}
