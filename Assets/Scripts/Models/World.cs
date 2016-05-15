using UnityEngine;
using System.Collections.Generic;
using System;


public class World
{
    public Tile[,] Tiles { get; protected set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    private Dictionary<string, Furniture> _furniturePrototypes;

    private Action<Furniture> cbOnFurniturePlaced;


    public World(int width = 100, int height = 100)
    {
        Width = width;
        Height = height;

        Tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (Width * Height) + " tiles.");

        CreateFurniturePrototypes();
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return null;
        }

        return Tiles[x, y];
    }

    public void RandomizeTiles()
    {
        Debug.Log("RandomizeTiles");

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int val = UnityEngine.Random.Range(0, 2);

                if (val == 0)
                {
                    Tiles[x, y].Type = TileType.Empty;
                }
                else
                {
                    Tiles[x, y].Type = TileType.Floor;
                }
            }
        }
    }

    public void PlaceFurniture(string objectType, Tile tile)
    {
        // TODO: This assumes 1x1 tiles with no rotation
        if (!_furniturePrototypes.ContainsKey(objectType))
        {
            Debug.LogError("PlaceFurniture - Unable to place furniture, key doesn't exists!");
            return;
        }

        Furniture obj = Furniture.PlaceFurnitureInstance(_furniturePrototypes[objectType], tile);

        if (obj == null)
        {
            return;
        }

        if (cbOnFurniturePlaced != null)
        {
            cbOnFurniturePlaced(obj);
        }
    }

    public void RegisterOnFurniturePlaced(Action<Furniture> callback)
    {
        cbOnFurniturePlaced += callback;
    }

    public void UnregisterOnFurniturePlaced(Action<Furniture> callback)
    {
        cbOnFurniturePlaced -= callback;
    }


    private void CreateFurniturePrototypes()
    {
        _furniturePrototypes = new Dictionary<string, Furniture>();
        _furniturePrototypes.Add("Wall", Furniture.CreatePrototype( furnitureType: "Wall",
                                                                    movementCost: 0,
                                                                    width: 1,
                                                                    height: 1,
                                                                    linksToNeighbor: true));
    }
}
