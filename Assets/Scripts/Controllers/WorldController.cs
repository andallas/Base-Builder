using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }
    public World WorldData { get; protected set; }

    // TODO: Use a key/value pair of sprites, keyed on Tile.TileType
    public Sprite floorSprite;

    private Dictionary<Tile, GameObject> tileGameObjectMap;


	void Start()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found extra world controller.");
        }
        Instance = this;

        WorldData = new World();
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

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

                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
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
            tile_data.UnregisterTileTypeChangedCallback(OnTileTypeChanged);
            Destroy(tile_go);
        }
    }

    void OnTileTypeChanged(Tile tile_data)
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
}
