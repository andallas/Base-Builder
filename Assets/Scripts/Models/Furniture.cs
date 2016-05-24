using UnityEngine;
using System.Collections;
using System;


public class Furniture
{
    public Tile Tile { get; protected set; }
    public string Type { get; protected set; }
    public float MovementCost { get; protected set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public bool LinksToNeighbor { get; protected set; }

    private Action<Furniture> cbOnChanged;


    // TODO: Implement larger objects
    // TODO: Implement object rotation


    protected Furniture() { }


    static public Furniture CreatePrototype(string furnitureType,
                                            float movementCost = 1f,
                                            int width = 1,
                                            int height = 1,
                                            bool linksToNeighbor = false)
    {
        Furniture furniture = new Furniture();

        furniture.Type              = furnitureType;
        furniture.MovementCost      = movementCost;
        furniture.Width             = width;
        furniture.Height            = height;
        furniture.LinksToNeighbor   = linksToNeighbor;

        return furniture;
    }

    static public Furniture PlaceFurnitureInstance(Furniture proto, Tile tile)
    {
        Furniture furniture = new Furniture();

        furniture.Type              = proto.Type;
        furniture.MovementCost      = proto.MovementCost;
        furniture.Width             = proto.Width;
        furniture.Height            = proto.Height;
        furniture.LinksToNeighbor   = proto.LinksToNeighbor;
        furniture.Tile = tile;

        if (!tile.InstallFurniture(furniture))
        {
            return null;
        }

        if (furniture.LinksToNeighbor)
        {
            int x = tile.X;
            int y = tile.Y;

            Tile t;
            t = tile.World.GetTileAt(x, y + 1);
            if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }

            t = tile.World.GetTileAt(x + 1, y);
            if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }

            t = tile.World.GetTileAt(x, y - 1);
            if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }

            t = tile.World.GetTileAt(x - 1, y);
            if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }
        }

        return furniture;
    }


    public void RegisterOnChangedCallback(Action<Furniture> callback)
    {
        cbOnChanged += callback;
    }

    public void UnregisterOnChangedCallback(Action<Furniture> callback)
    {
        cbOnChanged -= callback;
    }
}
