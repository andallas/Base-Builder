using UnityEngine;
using System.Collections.Generic;


public class Room
{
    public float Atmosphere_O2 { get; protected set; }
    public float Atmosphere_N { get; protected set; }
    public float Atmosphere_CO2 { get; protected set; }

    private List<Tile> _tiles;

    public Room()
    {
        _tiles = new List<Tile>();
    }

    public void AssignTile(Tile tile)
    {
        if (_tiles.Contains(tile))
        {
            return;
        }

        tile.ParentRoom = this;
        _tiles.Add(tile);
    }

    public void UnassignAllTiles()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].ParentRoom = WorldController.WorldData.Outside;
        }

        _tiles = new List<Tile>();
    }

    static public void DoRoomFloodFill(Furniture sourceFurniture)
    {
        // sourceFurniture is the piece of furniture that may be
        // splitting two eisting rooms, or may be the final
        // enclosing piece to form a new room.
        // Check the NESW neighbors of the furniture's tile
        // and do flood fill from them

        // If this piece of furniture was added to an existing room
        // (which should always be true assuming we consider 'outside' to be a big room)
        // delete that room and assign all tiles within to be 'outside' for now

        if (sourceFurniture.Tile.ParentRoom != WorldController.WorldData.Outside)
        {
            WorldController.WorldData.DeleteRoom(sourceFurniture.Tile.ParentRoom);
        }
    }
}
