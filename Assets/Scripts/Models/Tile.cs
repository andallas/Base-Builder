using UnityEngine;
using System.Collections;
using System;

public enum TileType { Empty, Floor };

public class Tile
{
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

    public bool InstallObject(InstalledObject obj)
    {
        if (_installedObject != null)
        {
            Debug.LogError("InstallObject - Tried to assign an installed object to a tile that already has one!");
            return false;
        }

        _installedObject = obj;
        return true;
    }
}
