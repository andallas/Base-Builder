﻿using System.Collections.Generic;
using UnityEngine;


public class FurnitureSpriteController : MonoBehaviour
{
	public Dictionary<string, Sprite> furnitureSprites;

	private Dictionary<Furniture, GameObject> furnitureGameObjectMap;


	void Start()
	{
		LoadSprites();

		furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

		WorldController.WorldData.RegisterOnFurnitureCreated(OnFurnitureCreated);

		foreach (Furniture furniture in WorldController.WorldData.Furnishings)
		{
			OnFurnitureCreated(furniture);
		}
	}


	public void OnFurnitureCreated(Furniture furniture)
	{
		// TODO: Does not consider multi-tile nor rotated character.
		int x = furniture.Tile.X;
		int y = furniture.Tile.Y;

		GameObject furniture_go = new GameObject();
		furniture_go.name = furniture.Type + "_" + x + "_" + y;
		furniture_go.transform.position = new Vector3(x, y, 0);
		furniture_go.transform.SetParent(transform, true);

		SpriteRenderer furniture_sr = furniture_go.AddComponent<SpriteRenderer>();
		furniture_sr.sprite = GetSpriteForFurniture(furniture);
		furniture_sr.sortingLayerName = "Furniture";

		furnitureGameObjectMap[furniture] = furniture_go;

		furniture.RegisterOnChangedCallback(OnFurnitureChanged);
	}

	public Sprite GetSpriteForFurniture(Furniture furniture)
	{
		int x = furniture.Tile.X;
		int y = furniture.Tile.Y;

		if (!furniture.LinksToNeighbor)
		{
			if (!furnitureSprites.ContainsKey(furniture.Type))
			{
				Debug.LogError("GetSpriteForFurniture - No sprites with name: " + furniture.Type);
			}
			return furnitureSprites[furniture.Type];
		}

		string spriteName = furniture.Type + "_";

		Tile t;
		t = WorldController.WorldData.GetTileAt(x, y + 1);
		if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
		{
			spriteName += "N";
		}

		t = WorldController.WorldData.GetTileAt(x + 1, y);
		if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
		{
			spriteName += "E";
		}

		t = WorldController.WorldData.GetTileAt(x, y - 1);
		if (t != null && t.Furniture != null && t.Furniture.Type == furniture.Type)
		{
			spriteName += "S";
		}

		t = WorldController.WorldData.GetTileAt(x - 1, y);
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

	public Sprite GetSpriteForFurniture(string furnitureType)
	{
		if (furnitureSprites.ContainsKey(furnitureType))
		{
			return furnitureSprites[furnitureType];
		}

		if (furnitureSprites.ContainsKey(furnitureType + "_"))
		{
			return furnitureSprites[furnitureType + "_"];
		}

		Debug.LogError("GetSpriteForFurniture - No sprites with name: " + furnitureType);
		return null;
	}


	private void LoadSprites()
	{
		furnitureSprites = new Dictionary<string, Sprite>();
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture/");

		foreach (Sprite s in sprites)
		{
			furnitureSprites[s.name] = s;
		}
	}

	private void OnFurnitureChanged(Furniture furniture)
	{
		// TODO: Make sure the furniture graphics have been updated
		if (!furnitureGameObjectMap.ContainsKey(furniture))
		{
			Debug.LogError("OnFurnitureChanged - trying to change visuals for furniture, not found in map.");
			return;
		}
		GameObject furniture_go = furnitureGameObjectMap[furniture];

		if (furniture_go == null)
		{
			Debug.LogError("WTF?!?!?!");
		}
		SpriteRenderer spriteRenderer = furniture_go.GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
		{
			spriteRenderer = furniture_go.AddComponent<SpriteRenderer>();
		}
		spriteRenderer.sprite = GetSpriteForFurniture(furniture);
	}
}
