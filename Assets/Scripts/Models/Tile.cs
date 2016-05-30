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

    // TODO: Implement EnvironmentalFactor.MovementCost and other movement prohibitors/enablers
    // This is a multiplier. So a value of "2" here means you move twice as slowly (i.e. at half speed). Tile types and other
    // environmentla effects may be combined. For example; a "rough" tile (cost of 2) with a table furniture (cost of 3)
    // that is on fire (cost of 3) would have a total movement cost of (2 + 3 + 3 = 8), so  you'd move through this tile
    // at 1/8th normal speed.
    // NOTE: If MovementCost == 0, then this tile is impassible. (e.g. a wall).
    public float MovementCost
    {
        get
        {
            //float tileEnvironmentalMultiplier = (EnvironmentalFactor != null) ? EnvironmentalFactor.MovementCost : 0;
            float tileFurnitureMultiplier = (Furniture != null) ? Furniture.MovementCost : 1;
            float tileTypeMultiplier = (Type != TileType.Empty) ? 1 : 0;

            return
                //tileEnvironmentalMultiplier *
                tileFurnitureMultiplier *
                tileTypeMultiplier;
        }
    }

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
            // Tried to assign character to a tile that already has one!
            return false;
        }

        Furniture = furnitureInstance;
        return true;
    }

    public bool IsNeighbor(Tile tile, bool diagonalOk = false)
    {
        // SAVE: Keeping this comment block here to explain what this line means:
        //       return (Mathf.Abs(X - tile.X) + Mathf.Abs(Y - tile.Y) == 1 || (diagonalOk && (Mathf.Abs(X - tile.X) == 1 && Mathf.Abs(Y - tile.Y) == 1)));

        // I really like how this turned out :)
        //int rowOffset = Mathf.Abs(X - tile.X);
        //int colOffset = Mathf.Abs(Y - tile.Y);

        //bool aboveOrBelowRow = rowOffset == 1;
        //bool aboveOrBelowCol = colOffset == 1;

        //bool isCardinalNeighbor = rowOffset + colOffset == 1;
        //bool isDiagonalNeighbor = aboveOrBelowRow && aboveOrBelowCol;

        //return (isCardinalNeighbor || (diagonalOk && isDiagonalNeighbor));


        return (Mathf.Abs(X - tile.X) + Mathf.Abs(Y - tile.Y) == 1 || (diagonalOk && (Mathf.Abs(X - tile.X) == 1 && Mathf.Abs(Y - tile.Y) == 1)));
    }

    public Tile[] GetNeighbors(bool diagonalOk = false, bool clippingOk = true)
    {
        // TODO: Figure out how to cache this so we don't have to do these calculations so often.
        Tile[] neighbors;

        if (diagonalOk)
        {
            neighbors = new Tile[8];
            neighbors[4] = World.GetTileAt(TilePosition.NorthEast(X, Y));
            neighbors[5] = World.GetTileAt(TilePosition.SouthEast(X, Y));
            neighbors[6] = World.GetTileAt(TilePosition.SouthWest(X, Y));
            neighbors[7] = World.GetTileAt(TilePosition.NorthWest(X, Y));
        }
        else
        {
            neighbors = new Tile[4];
        }
        
        neighbors[0] = World.GetTileAt(TilePosition.North(X, Y));
        neighbors[1] = World.GetTileAt(TilePosition.East(X, Y));
        neighbors[2] = World.GetTileAt(TilePosition.South(X, Y));
        neighbors[3] = World.GetTileAt(TilePosition.West(X, Y));

        return neighbors;
    }
}
