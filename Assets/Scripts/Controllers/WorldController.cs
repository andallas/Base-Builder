﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }
    public World WorldData { get; protected set; }

    // TODO: Use a key/value pair of sprites, keyed on Tile.TileType
    public Sprite floorSprite;
    public Dictionary<string, Sprite> furnitureSprites;

    private Dictionary<Tile, GameObject> tileGameObjectMap;
    private Dictionary<Furniture, GameObject> furnitureGameObjectMap;


    void Start()
    {
        // Sprite loading
        furnitureSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture/Wall");

        foreach (Sprite s in sprites)
        {
            furnitureSprites[s.name] = s;
        }
        
        if (Instance != null)
        {
            Debug.LogWarning("Found extra world controller.");
        }
        Instance = this;

        WorldData = new World();
        WorldData.RegisterOnFurniturePlaced(OnFurniturePlaced);

        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        for (int x = 0; x < WorldData.Width; x++)
        {
            for (int y = 0; y < WorldData.Height; y++)
            {
                Tile tile_data = WorldData.GetTileAt(x, y);

                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);
                tile_go.AddComponent<SpriteRenderer>();

                tileGameObjectMap[tile_data] = tile_go;

                tile_data.RegisterOnTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        WorldData.RandomizeTiles();
	}

    void Update() { }


    private void DestroyAllTileGameObjects()
    {
        while (tileGameObjectMap.Count > 0)
        {
            Tile tile_data = tileGameObjectMap.Keys.First();
            GameObject tile_go = tileGameObjectMap[tile_data];

            tileGameObjectMap.Remove(tile_data);
            tile_data.UnregisterOnTileTypeChangedCallback(OnTileTypeChanged);
            Destroy(tile_go);
        }
    }

    private void OnTileTypeChanged(Tile tile_data)
    {
        if (!tileGameObjectMap.ContainsKey(tile_data))
        {
            Debug.LogError("OnTileTypeChanged - tile_data not found!");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null)
        {
            Debug.LogError("OnTileTypeChanged - tile_go is null!");
            return;
        }

        // TODO: Consider changing this to a switch statement
        if (tile_data.Type == TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
        }
    }


    public Tile GetTileAtWorldCoord(Vector3 vec)
    {
        int x = Mathf.FloorToInt(vec.x);
        int y = Mathf.FloorToInt(vec.y);

        return Instance.WorldData.GetTileAt(x, y);
    }

    public void OnFurniturePlaced(Furniture furniture)
    {
        // TODO: Does not consider multi-tile nor rotated furniture.

        GameObject furniture_go = new GameObject();
        int x = furniture.Tile.X;
        int y = furniture.Tile.Y;
        furniture_go.name = furniture.Type + "_" + x + "_" + y;
        furniture_go.transform.position = new Vector3(x, y, 0);
        furniture_go.transform.SetParent(transform, true);

        furniture_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furniture);

        furnitureGameObjectMap[furniture] = furniture_go;

        furniture.RegisterOnChangedCallback(OnFurnitureChanged);
    }

    public void OnFurnitureChanged(Furniture obj)
    {
        Debug.Log("OnFurnitureChanged!");
    }


    private Sprite GetSpriteForFurniture(Furniture furniture)
    {
        int x = furniture.Tile.X;
        int y = furniture.Tile.Y;

        if (!furniture.LinksToNeighbor)
        {
            return furnitureSprites[furniture.Type];
        }

        string spriteName = furniture.Type + "_";

        Tile t;
        t = WorldData.GetTileAt(x, y + 1);
        if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
        {
            spriteName += "N";
        }

        t = WorldData.GetTileAt(x + 1, y);
        if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
        {
            spriteName += "E";
        }

        t = WorldData.GetTileAt(x, y - 1);
        if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
        {
            spriteName += "S";
        }

        t = WorldData.GetTileAt(x - 1, y);
        if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
        {
            spriteName += "W";
        }

        if (!furnitureSprites.ContainsKey(spriteName))
        {
            Debug.LogError("GetSpriteForFurniture - No sprites with name: " + spriteName);
        }

        return furnitureSprites[spriteName];
    }
}
