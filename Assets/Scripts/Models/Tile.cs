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
                if (cbOnTileTypeChanged != null)
                {
                    cbOnTileTypeChanged(this);
                }
            }
        }
    }

    public int X { get; protected set; }
    public int Y { get; protected set; }


    public Inventory Inventory { get; protected set; }
    public Furniture Furniture { get; protected set; }
    public World World { get; protected set; }
    private Action<Tile> cbOnTileTypeChanged;


    public Tile(World world, int x, int y)
    {
        World = world;
        X = x;
        Y = y;
    }

    public void RegisterOnTileTypeChangedCallback(Action<Tile> callback)
    {
        cbOnTileTypeChanged += callback;
    }

    public void UnregisterOnTileTypeChangedCallback(Action<Tile> callback)
    {
        cbOnTileTypeChanged -= callback;
    }

    public bool InstallFurniture(Furniture furniture)
    {
        if (Furniture != null)
        {
            // Tried to assign furniture to a tile that already has one!
            return false;
        }

        Furniture = furniture;
        return true;
    }
}
