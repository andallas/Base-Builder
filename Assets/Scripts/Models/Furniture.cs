using System;
using UnityEngine;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;


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
    public Dictionary<string, object> FurnitureParameters { get; protected set; }
    public object FurnitureParameter(string key)
    {
        if (FurnitureParameters != null && FurnitureParameters.ContainsKey(key))
        {
            return FurnitureParameters[key];
        }

        return null;
    }
    public void FurnitureParameter(string key, object value)
    {
        if (FurnitureParameters != null && FurnitureParameters.ContainsKey(key))
        {
            FurnitureParameters[key] = value;
        }
    }

    public Action<Furniture, float> UpdateActions;

    private Action<Furniture> cbOnChanged;
    private Func<Tile, bool> funcPositionValidation;
    

    // TODO: Implement objects that take up more than 1 tile space
    // TODO: Implement object rotation

    // Create furniture from parameters -- this will probably ONLY ever be used for prototypes
    public Furniture(string furnitureType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false)
    {
        Type                    = furnitureType;
        MovementCost            = movementCost;
        Width                   = width;
        Height                  = height;
        LinksToNeighbor         = linksToNeighbor;
        FurnitureParameters     = new Dictionary<string, object>();

        funcPositionValidation = IsValidPosition_Base;
    }

    protected Furniture(Furniture other)
    {
        Type                    = other.Type;
		MovementCost            = other.MovementCost;
		Width                   = other.Width;
		Height                  = other.Height;
		LinksToNeighbor         = other.LinksToNeighbor;
        FurnitureParameters     = new Dictionary<string, object>(other.FurnitureParameters);

        if (other.UpdateActions != null)
        {
            UpdateActions = (Action<Furniture, float>)other.UpdateActions.Clone();
        }
    }
    

	static public Furniture PlaceInstance(Furniture proto, Tile tile)
	{
		if (!proto.funcPositionValidation(tile))
		{
			Debug.LogError("PlaceInstance - Position is invalid");
			return null;
		}

		Furniture furniture = proto.Clone();
		furniture.Tile = tile;

		if (!tile.InstallFurniture(furniture))
		{
			return null;
		}

        if (furniture.LinksToNeighbor)
        {
            UpdateNeighbors(tile.X, tile.Y, furniture.Type);
        }

		return furniture;
	}

    static private void UpdateNeighbors(int x, int y, string furnitureType)
    {
        World world = WorldController.WorldData;
        UpdateNeighborAtTile(world.GetTileAt(x, y + 1), furnitureType);
        UpdateNeighborAtTile(world.GetTileAt(x + 1, y), furnitureType);
        UpdateNeighborAtTile(world.GetTileAt(x, y - 1), furnitureType);
        UpdateNeighborAtTile(world.GetTileAt(x - 1, y), furnitureType);
    }

    static private void UpdateNeighborAtTile(Tile t, string furnitureType)
    {
        if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.Type == furnitureType)
        {
            t.Furniture.cbOnChanged(t.Furniture);
        }
    }


    virtual public Furniture Clone()
    {
        return new Furniture(this);
    }

    public void Update(float deltaTime)
    {
        // TODO: Get C#6 working with Unity so we can do this instead:
        //       _updateActions?.Invoke(this, deltaTime);
        if (UpdateActions != null)
        {
            UpdateActions(this, deltaTime);
        }
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
		return !(tile.Type != TileType.Floor &&
                tile.Furniture != null);
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
