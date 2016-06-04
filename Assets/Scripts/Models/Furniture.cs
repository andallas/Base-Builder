using System;
using UnityEngine;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


public class Furniture : IXmlSerializable
{
	public Tile Tile { get; protected set; }
	public string Type { get; protected set; }
	public int Width { get; protected set; }
	public int Height { get; protected set; }
	public bool LinksToNeighbor { get; protected set; }
	// This is a multiplier. So a value of "2" here means you move twice as slowly (i.e. at half speed). Tile types and other
	// environmentla effects may be combined. For example; a "rough" tile (cost of 2) with a table furniture (cost of 3)
	// that is on fire (cost of 3) would have a total movement cost of (2 + 3 + 3 = 8), so  you'd move through this tile
	// at 1/8th normal speed. NOTE: If MovementCost == 0, then this tile is impassible. (e.g. a wall).
	public float MovementCost { get; protected set; }


	private Action<Furniture> cbOnChanged;
	private Func<Tile, bool> funcPositionValidation;


	// TODO: Implement objects that take up more than 1 tile space
	// TODO: Implement object rotation


	static public Furniture CreatePrototype(string furnitureType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false)
	{
		Furniture furniture = new Furniture();

		furniture.Type              = furnitureType;
		furniture.MovementCost      = movementCost;
		furniture.Width             = width;
		furniture.Height            = height;
		furniture.LinksToNeighbor   = linksToNeighbor;

		furniture.funcPositionValidation = furniture.IsValidPosition_Base;

		return furniture;
	}

	static public Furniture PlaceInstance(Furniture proto, Tile tile)
	{
		if (!proto.funcPositionValidation(tile))
		{
			Debug.LogError("PlaceInstance - Position is invalid");
			return null;
		}

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
			World world = WorldController.WorldData;

			Tile t;
			t = world.GetTileAt(x, y + 1);
			if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.Type == furniture.Type)
			{
				t.Furniture.cbOnChanged(t.Furniture);
			}

			t = world.GetTileAt(x + 1, y);
			if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.Type == furniture.Type)
			{
				t.Furniture.cbOnChanged(t.Furniture);
			}

			t = world.GetTileAt(x, y - 1);
			if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.Type == furniture.Type)
			{
				t.Furniture.cbOnChanged(t.Furniture);
			}

			t = world.GetTileAt(x - 1, y);
			if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.Type == furniture.Type)
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

	public bool IsValidPosition(Tile tile)
	{
		return funcPositionValidation(tile);
	}


	private bool IsValidPosition_Base(Tile tile)
	{
		if (tile.Type != TileType.Floor)
		{
			return false;
		}

		if (tile.Furniture != null)
		{
			return false;
		}
		return true;
	}

	private bool IsValidPosition_Door(Tile tile)
	{
		if (!IsValidPosition_Base(tile))
		{
			return false;
		}

		// TODO: Check tile has E/W or N/S walls
		return true;
	}


	#region Saving & Loading
	public XmlSchema GetSchema() { return null; }

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteStartElement("Furniture");
			writer.WriteAttributeString("X", Tile.X.ToString());
			writer.WriteAttributeString("Y", Tile.Y.ToString());
			writer.WriteAttributeString("Type", Type.ToString());
			writer.WriteAttributeString("MovementCost", MovementCost.ToString());
		writer.WriteEndElement();
	}

	public void ReadXml(XmlReader reader)
	{
		MovementCost = float.Parse(reader.GetAttribute("MovementCost"));
	}
	#endregion
}
