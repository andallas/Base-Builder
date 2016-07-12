using System.Collections.Generic;
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

		foreach (Furniture furniture in WorldController.WorldData.Furnitures)
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

        // TODO: This hardcoding is not ideal!
        if (furniture.Type == "Door")
        {
            // By default, the door graphic is meant for walls to the E/W
            // Check to see if we actually have a wall N/S, and if so then
            // rotate this game object by 90 degrees

            Tile N = WorldController.WorldData.GetTileAt(furniture.Tile.X, furniture.Tile.Y + 1);
            Tile S = WorldController.WorldData.GetTileAt(furniture.Tile.X, furniture.Tile.Y - 1);
            Tile E = WorldController.WorldData.GetTileAt(furniture.Tile.X + 1, furniture.Tile.Y);
            Tile W = WorldController.WorldData.GetTileAt(furniture.Tile.X - 1, furniture.Tile.Y);

            if (N != null && S != null && N.Furniture != null && S.Furniture != null && N.Furniture.Type == "Wall" && S.Furniture.Type == "Wall")
            {
                furniture_go.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }

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

        string spriteName = furniture.Type + "_";

        if (!furniture.LinksToNeighbor)
		{
            // If this is a door, let's check 'openPercentage' and update the sprite
            if (furniture.Type == "Door")
            {
                float openPercentage = furniture.FurnitureParameter("openPercentage");
                if (openPercentage <= 0.1f)
                {
                    // Door is closed
                    spriteName = "Door";
                }
                else if (openPercentage <= 0.5f)
                {
                    // Door is slightly open
                    spriteName = "Door_SlightlyOpen";
                }
                else if (openPercentage <= 0.9f)
                {
                    // Door is mostly open
                    spriteName = "Door_MostlyOpen";
                }
                else
                {
                    // Door is fully open
                    spriteName = "Door_FullyOpen";
                }
            }
            
            if (!furnitureSprites.ContainsKey(spriteName))
			{
				Debug.LogErrorFormat("GetSpriteForFurniture - No sprites with name: {0}", spriteName);
			}

			return furnitureSprites[spriteName];
		}

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
