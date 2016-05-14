using UnityEngine;
using System.Collections;

public class InstalledObject
{
    private Tile _tile;
    private string _objectType;
    public string ObjectType { get; protected set; }
    private float _movementCost;
    public float MovementCost { get; protected set; }
    private int _width;
    public int Width { get; protected set; }
    private int _height;
    public int Height { get; protected set; }

    // TODO: Implement larger objects
    // TODO: Implement object rotation

    protected InstalledObject()
    {

    }

    static public InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        InstalledObject obj = new InstalledObject();

        obj._objectType     = objectType;
        obj._movementCost   = movementCost;
        obj._width          = width;
        obj._height         = height;

        return obj;
    }

    static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        InstalledObject obj = new InstalledObject();

        obj._objectType     = proto.ObjectType;
        obj._movementCost   = proto.MovementCost;
        obj._width          = proto.Width;
        obj._height         = proto.Height;

        obj._tile = tile;

        if (!tile.InstallObject(obj))
        {
            return null;
        }

        return obj;
    }
}
