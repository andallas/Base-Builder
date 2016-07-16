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

        if (tile.ParentRoom != null)
        {
            tile.ParentRoom._tiles.Remove(tile);
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

    protected static void FloodFill(Tile tile, Room oldRoom)
    {
        if (tile == null && tile.ParentRoom != oldRoom &&
            (tile.Furniture == null || tile.Furniture.RoomEnclosure) ||
            tile.Type == TileType.Empty)
        {
            return;
        }

        Room potentialRoom = new Room();
        var tilesToCheck = new Queue<Tile>();
        tilesToCheck.Enqueue(tile);

        while (tilesToCheck.Count > 0)
        {
            Tile t = tilesToCheck.Dequeue();
            
            if (t.ParentRoom == oldRoom)
            {
                potentialRoom.AssignTile(t);

                Tile[] neighbors = t.GetNeighbors();
                foreach (Tile t2 in neighbors)
                {
                    // Open space
                    if (t2 == null || t2.Type == TileType.Empty)
                    {
                        potentialRoom.UnassignAllTiles();
                        return;
                    }

                    if (t2.ParentRoom == oldRoom &&
                        (t2.Furniture == null || !t2.Furniture.RoomEnclosure))
                    {
                        tilesToCheck.Enqueue(t2);
                    }
                }
            }
        }

        WorldController.WorldData.AddRoom(potentialRoom);
    }

    public static void DoRoomFloodFill(Furniture sourceFurniture)
    {
        // sourceFurniture is the piece of furniture that may be
        // splitting two eisting rooms, or may be the final
        // enclosing piece to form a new room.
        // Check the NESW neighbors of the furniture's tile
        // and do flood fill from them

        // If this piece of furniture was added to an existing room
        // (which should always be true assuming we consider 'outside' to be a big room)
        // delete that room and assign all tiles within to be 'outside' for now
        Tile sourceTile = sourceFurniture.Tile;
        Room oldRoom = sourceTile.ParentRoom;

        foreach (Tile t in sourceTile.GetNeighbors())
        {
            FloodFill(t, oldRoom);
        }

        sourceFurniture.Tile.ParentRoom = null;
        oldRoom._tiles.Remove(sourceTile);

        if (oldRoom != WorldController.WorldData.Outside)
        {
            if (oldRoom._tiles.Count > 0)
            {
                Debug.LogError("'oldRoom' still has tiles assigned to it!");
            }
            WorldController.WorldData.DeleteRoom(oldRoom);
        }
    }
}
