using UnityEngine;
using System.Collections;
using System;

public class Tile
{
    public enum TileType { Empty, Floor };


    private TileType _type = TileType.Empty;
    public TileType Type
    {
        get { return _type; }
        set
        {
            if (_type != value)
            {
                _type = value;
                if (cbTileTypeChanged != null)
                {
                    cbTileTypeChanged(this);
                }
            }
        }
    }

    private int _x;
    public int X
    {
        get { return _x; }
        protected set { _x = value; }
    }

    private int _y;
    public int Y
    {
        get { return _y; }
        protected set { _y = value; }
    }


    private LooseObject _looseObject;
    private InstalledObject _installedObject;
    private World _world;
    private Action<Tile> cbTileTypeChanged;


    public Tile(World world, int x, int y)
    {
        _world = world;
        _x = x;
        _y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }
}
