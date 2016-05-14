using UnityEngine;
using System.Collections;

public class World
{
    private Tile[,] _tiles;
    public Tile[,] Tiles { get { return _tiles; } }

    private int _width;
    public int Width { get { return _width; } }

    private int _height;
    public int Height { get { return _height; } }

    public World(int width = 100, int height = 100)
    {
        _width = width;
        _height = height;

        _tiles = new Tile[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (_width * _height) + " tiles.");
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
        {
            return null;
        }

        return _tiles[x, y];
    }

    public void RandomizeTiles()
    {
        Debug.Log("RandomizeTiles");

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                int val = Random.Range(0, 2);

                if (val == 0)
                {
                    _tiles[x, y].Type = Tile.TileType.Empty;
                }
                else
                {
                    _tiles[x, y].Type = Tile.TileType.Floor;
                }
            }
        }
    }
}
