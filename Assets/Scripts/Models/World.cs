using UnityEngine;
using System.Collections.Generic;
using System;


public class World
{
    public Tile[,] Tiles { get; protected set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    private List<Character> characters;
    private Dictionary<string, Furniture> _furniturePrototypes;

    private Action<Furniture> cbOnFurniturePlaced;
    private Action<Tile> cbOnTileChanged;
    private Action<Character> cbOnCharacterCreated;


    public JobQueue jobQueue;


    public World(int width = 100, int height = 100)
    {
        jobQueue = new JobQueue();

        Width = width;
        Height = height;

        Tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y] = new Tile(this, x, y);
                Tiles[x, y].RegisterOnTileChangedCallback(OnTileChanged);
            }
        }

        CreateFurniturePrototypes();

        characters = new List<Character>();
    }

    public void CreateCharacter(Tile tile)
    {
        Character character = new Character(Tiles[Width / 2, Height / 2]);

        if (cbOnCharacterCreated != null)
        {
            cbOnCharacterCreated(character);
        }

        characters.Add(character);
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

    public void PlaceFurniture(string furnitureType, Tile tile)
    {
        // TODO: This assumes 1x1 tiles with no rotation
        if (!_furniturePrototypes.ContainsKey(furnitureType))
        {
            Debug.LogError("PlaceFurniture - Unable to place character, key doesn't exists!");
            return;
        }

        Furniture furniture = Furniture.PlaceFurnitureInstance(_furniturePrototypes[furnitureType], tile);

        if (furniture == null)
        {
            return;
        }

        if (cbOnFurniturePlaced != null)
        {
            cbOnFurniturePlaced(furniture);
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

    public void RegisterOnTileChanged(Action<Tile> callback)
    {
        cbOnTileChanged += callback;
    }

    public void UnregisterOnTileChanged(Action<Tile> callback)
    {
        cbOnTileChanged -= callback;
    }

    public void RegisterOnCharacterCreated(Action<Character> callback)
    {
        cbOnCharacterCreated += callback;
    }

    public void UnregisterOnCharacterCreated(Action<Character> callback)
    {
        cbOnCharacterCreated -= callback;
    }

    public Furniture GetFurniturePrototype(string furnitureType)
    {
        if (_furniturePrototypes.ContainsKey(furnitureType))
        {
            return _furniturePrototypes[furnitureType];
        }

        Debug.LogError("GetFurniturePrototype - Unknown character type: " + furnitureType);
        return null;
    }

    
    private void OnTileChanged(Tile tile)
    {
        if (cbOnTileChanged == null)
        {
            return;
        }
        cbOnTileChanged(tile);
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

    public bool IsFurniturePlacementValid(string furnitureType, Tile tile)
    {
        bool result = false;
        if (_furniturePrototypes.ContainsKey(furnitureType))
        {
            result = _furniturePrototypes[furnitureType].IsValidPosition(tile);
        }

        return result;
    }
}
