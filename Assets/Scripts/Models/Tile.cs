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
                if (cbOnTileChanged != null)
                {
                    cbOnTileChanged(this);
                }
            }
        }
    }

    public int X { get; protected set; }
    public int Y { get; protected set; }
    public Inventory Inventory { get; protected set; }
    public Furniture Furniture { get; protected set; }
    public World World { get; protected set; }
    public Job PendingFurnitureJob { get; set; }

    private Action<Tile> cbOnTileChanged;


    public Tile(World world, int x, int y)
    {
        World = world;
        X = x;
        Y = y;
    }

    public void RegisterOnTileChangedCallback(Action<Tile> callback)
    {
        cbOnTileChanged += callback;
    }

    public void UnregisterOnTileChangedCallback(Action<Tile> callback)
    {
        cbOnTileChanged -= callback;
    }

    public bool InstallFurniture(Furniture furnitureInstance)
    {
        if (Furniture != null)
        {
            // Tried to assign furniture to a tile that already has one!
            return false;
        }

        Furniture = furnitureInstance;
        return true;
    }
}
