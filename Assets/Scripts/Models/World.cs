﻿using Pathfinding;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;


public class World : IXmlSerializable
{
	public int Width { get; protected set; }
	public int Height { get; protected set; }
    public Room Outside { get { return Rooms[0]; } }
    public Tile[,] Tiles { get; protected set; }
	public List<Furniture> Furnitures { get; protected set; }
	public List<Character> Characters { get; protected set; }
	public List<Room> Rooms { get; protected set; }
	private Graph _tileGraph;
	public Graph TileGraph
	{
		get
		{
			if (_tileGraph == null)
			{
				_tileGraph = new Graph(WorldController.WorldData);
			}

			return _tileGraph;
		}
	}

	private Dictionary<string, Furniture> _furniturePrototypes;

	private Action<Furniture> cbOnFurnitureCreated;
	private Action<Tile> cbOnTileChanged;
	private Action<Character> cbOnCharacterCreated;


	public JobQueue jobQueue;

	public World() { }

	public World(int width, int height)
	{
		Width = width;
		Height = height;

		InitializeWorld();

        CreateCharacter(GetTileAt(Width / 2, Height / 2));
	}

	private void InitializeWorld()
	{
		jobQueue = new JobQueue();

		Tiles = new Tile[Width, Height];

        Rooms = new List<Room>();
        // Create the outside
        Rooms.Add(new Room());

        for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				Tiles[x, y] = new Tile(x, y);
				Tiles[x, y].RegisterOnTileChangedCallback(OnTileChanged);
                // Rooms 0 is a reserved room and the "Outside" room
                Tiles[x, y].ParentRoom = Outside;

            }
		}

		CreateFurniturePrototypes();

		Characters = new List<Character>();
		Furnitures = new List<Furniture>();
	}


	public void Update(float deltaTime)
	{
		foreach (Character character in Characters)
		{
			character.Update(deltaTime);
		}

        foreach (Furniture furniture in Furnitures)
        {
            furniture.Update(deltaTime);
        }
    }

    public void AddRoom(Room room)
    {
        Rooms.Add(room);
    }

    public void DeleteRoom(Room room)
    {
        if (room == Outside)
        {
            Debug.LogError("Tried to delete the outside room");
            return;
        }

        Rooms.Remove(room);

        room.UnassignAllTiles();
    }

	public Character CreateCharacter(Tile tile)
	{
		Character character = new Character(tile);

		if (cbOnCharacterCreated != null)
		{
			cbOnCharacterCreated(character);
		}

		Characters.Add(character);

		return character;
	}

	public Tile GetTileAt(int x, int y)
	{
		if (x < 0 || x >= Width || y < 0 || y >= Height)
		{
			return null;
		}

		return Tiles[x, y];
	}

	public Tile GetTileAt(Vector2 position)
	{
		return GetTileAt((int)position.x, (int)position.y);
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

	public Furniture PlaceFurniture(string furnitureType, Tile tile)
	{
		// TODO: This assumes 1x1 tiles with no rotation
		if (!_furniturePrototypes.ContainsKey(furnitureType))
		{
			Debug.LogError("PlaceFurniture - Unable to place furniture, key doesn't exists!");
			return null;
		}

		Furniture furniture = Furniture.PlaceInstance(_furniturePrototypes[furnitureType], tile);

		if (furniture == null)
		{
			return null;
		}

		Furnitures.Add(furniture);

        // Do we need to recalculate our rooms?
        if (furniture.RoomEnclosure)
        {
            Room.DoRoomFloodFill(furniture);
        }

		if (cbOnFurnitureCreated != null)
		{
			cbOnFurnitureCreated(furniture);

            if (furniture.MovementCost != 1)
            {
                // Since tiles return movement cost as their base cost multiplied
                // by the furniture's movement cost, a furniture movement cost
                // of exactly 1 doesn't impact our pathfinding system, so we can
                // occasionally avoid invalidating pathfinding graphs
                InvalidateTileGraph();
            }
		}

		return furniture;
	}

	public void RegisterOnFurnitureCreated(Action<Furniture> callback)
	{
		cbOnFurnitureCreated += callback;
	}

	public void UnregisterOnFurnitureCreated(Action<Furniture> callback)
	{
		cbOnFurnitureCreated -= callback;
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

	public void InvalidateTileGraph()
	{
		_tileGraph = null;
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

	// TODO: For DEBUG use only
	public void SetupPathfindingExample()
	{
		int l = Width / 2 - 5;
		int b = Height / 2 - 5;

		for (int x = l - 5; x < l + 15; x++)
		{
			for (int y = b - 5; y < b + 15; y++)
			{
				Tiles[x, y].Type = TileType.Floor;

				if (x == l || x == (l + 9) ||
					y == b || y == (b + 9))
				{
					if (x != (l + 9) &&
						y != (b + 4))
					{
						PlaceFurniture("Wall", Tiles[x, y]);
					}
				}
			}
		}
	}


	private void OnTileChanged(Tile tile)
	{
		if (cbOnTileChanged == null)
		{
			return;
		}
		cbOnTileChanged(tile);

		InvalidateTileGraph();
	}

	private void CreateFurniturePrototypes()
	{
        // TODO: This function will be replaced by a function that reads all of our furniture data from a text file in the future.
		_furniturePrototypes = new Dictionary<string, Furniture>();
		_furniturePrototypes.Add("Wall", new Furniture(furnitureType: "Wall", movementCost: 0, width: 1, height: 1, linksToNeighbor: true, roomEnclosure: true));
        _furniturePrototypes.Add("Door", new Furniture(furnitureType: "Door", movementCost: 1, width: 1, height: 1, linksToNeighbor: false, roomEnclosure: true));

        if (_furniturePrototypes.ContainsKey("Door"))
        {
            _furniturePrototypes["Door"].FurnitureParameter("openPercentage", 0f);
            _furniturePrototypes["Door"].FurnitureParameter("isOpening", 0f);
            _furniturePrototypes["Door"].UpdateActions += FurnitureActions.Door_UpdateAction;
        }

        _furniturePrototypes["Door"].RequestEntrance = FurnitureActions.Door_RequestEntrance;
    }


	#region Saving & Loading
	public XmlSchema GetSchema() { return null; }

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteAttributeString("Width", Width.ToString());
		writer.WriteAttributeString("Height", Height.ToString());

        SaveTiles(writer);
        SaveFurnitures(writer);
        SaveCharacters(writer);
		
	}

    private void SaveTiles(XmlWriter writer)
    {
        writer.WriteStartElement("Tiles");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y].Type != TileType.Empty)
                {
                    Tiles[x, y].WriteXml(writer);
                }
            }
        }
        writer.WriteEndElement();
    }

    private void SaveFurnitures(XmlWriter writer)
    {
        writer.WriteStartElement("Furnitures");
        foreach (Furniture furniture in Furnitures)
        {
            furniture.WriteXml(writer);
        }
        writer.WriteEndElement();
    }

    private void SaveCharacters(XmlWriter writer)
    {
        writer.WriteStartElement("Characters");
        foreach (Character character in Characters)
        {
            character.WriteXml(writer);
        }
        writer.WriteEndElement();
    }

	public void ReadXml(XmlReader reader)
	{
		Width = int.Parse(reader.GetAttribute("Width"));
		Height = int.Parse(reader.GetAttribute("Height"));

		InitializeWorld();

		while (reader.Read())
		{
			switch (reader.Name)
			{
				case "Tiles":
					{
						LoadTiles(reader);
						break;
					}
				case "Furnitures":
					{
                        LoadFurnitures(reader);
						break;
					}
				case "Characters":
					{
						LoadCharacters(reader);
						break;
					}
			}
		}
	}

	private void LoadTiles(XmlReader reader)
	{
        if (reader.ReadToDescendant("Tile"))
        {
            do
            {
                int x = int.Parse(reader.GetAttribute("X"));
                int y = int.Parse(reader.GetAttribute("Y"));

                Tiles[x, y].ReadXml(reader);
            }
            while (reader.ReadToNextSibling("Tile"));
        }
	}

	private void LoadFurnitures(XmlReader reader)
	{
        if (reader.ReadToDescendant("Furniture"))
        {
            do
            {
                int x = int.Parse(reader.GetAttribute("X"));
                int y = int.Parse(reader.GetAttribute("Y"));

                Furniture furniture = PlaceFurniture(reader.GetAttribute("Type"), Tiles[x, y]);
                furniture.ReadXml(reader);
            }
            while (reader.ReadToNextSibling("Furniture"));
        }
	}

	private void LoadCharacters(XmlReader reader)
	{
        if (reader.ReadToDescendant("Character"))
        {
            do
            {
                int x = int.Parse(reader.GetAttribute("X"));
                int y = int.Parse(reader.GetAttribute("Y"));

                Character character = CreateCharacter(Tiles[x, y]);
                character.ReadXml(reader);
            }
            while (reader.ReadToNextSibling("Character"));
        }
	}
	#endregion
}
