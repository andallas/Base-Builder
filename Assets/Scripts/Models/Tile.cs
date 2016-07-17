using System;
using UnityEngine;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


public enum TileType { Empty, Floor };
public enum Enterability { Yes, Never, Soon };

public class Tile : IXmlSerializable
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
    public Enterability Enterable
    {
        get
        {
            // This returns true if you can enter this tile right this moment

            if (MovementCost == 0)
            {
                return Enterability.Never;
            }

            // Check our furniture to see if it has a special block on enterability
            if (Furniture != null && Furniture.RequestEntrance != null)
            {
                return Furniture.RequestEntrance(Furniture);
            }
            return Enterability.Yes;
        }
    }
    public int X { get; protected set; }
	public int Y { get; protected set; }
    public Tile North { get { return WorldController.WorldData.GetTileAt(X, Y + 1); } }
    public Tile South { get { return WorldController.WorldData.GetTileAt(X, Y - 1); } }
    public Tile East { get { return WorldController.WorldData.GetTileAt(X + 1, Y); } }
    public Tile West { get { return WorldController.WorldData.GetTileAt(X - 1, Y); } }
    public Inventory Inventory { get; protected set; }
	public Furniture Furniture { get; protected set; }
    public Job PendingFurnitureJob { get; set; }
    public Room ParentRoom { get; set; }

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
            // This would be for things such as a tile being on fire
			//float tileEnvironmentalMultiplier = (EnvironmentalFactor != null) ? EnvironmentalFactor.MovementCost : 0;
			float tileFurnitureMultiplier = (Furniture != null) ? Furniture.MovementCost : 1;
			float tileTypeMultiplier = (Type != TileType.Empty) ? 1 : 0;

			return /*tileEnvironmentalMultiplier **/ tileFurnitureMultiplier * tileTypeMultiplier;
		}
	}

	private Action<Tile> cbOnTileChanged;


	public Tile(int x, int y)
	{
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
		World world = WorldController.WorldData;

		if (diagonalOk)
		{
			neighbors = new Tile[8];
			neighbors[4] = world.GetTileAt(TilePosition.NorthEast(X, Y));
			neighbors[5] = world.GetTileAt(TilePosition.SouthEast(X, Y));
			neighbors[6] = world.GetTileAt(TilePosition.SouthWest(X, Y));
			neighbors[7] = world.GetTileAt(TilePosition.NorthWest(X, Y));
		}
		else
		{
			neighbors = new Tile[4];
		}

		neighbors[0] = world.GetTileAt(TilePosition.North(X, Y));
		neighbors[1] = world.GetTileAt(TilePosition.East(X, Y));
		neighbors[2] = world.GetTileAt(TilePosition.South(X, Y));
		neighbors[3] = world.GetTileAt(TilePosition.West(X, Y));

		return neighbors;
	}

    #region Saving & Loading
    public XmlSchema GetSchema() { return null; }

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteStartElement("Tile");
			writer.WriteAttributeString("X", X.ToString());
			writer.WriteAttributeString("Y", Y.ToString());
			writer.WriteAttributeString("Type", ((int)Type).ToString());
		writer.WriteEndElement();
	}

	public void ReadXml(XmlReader reader)
	{
		Type = (TileType)int.Parse(reader.GetAttribute("Type"));
	}
	#endregion
}
