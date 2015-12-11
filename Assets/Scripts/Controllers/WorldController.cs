using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }
    public World WorldData { get; protected set; }

    public Sprite floorSprite;


	void Start()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found extra world controller.");
        }
        Instance = this;

        WorldData = new World();

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

                tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );
            }
        }

        WorldData.RandomizeTiles();
	}

    void Update() { }

    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        if (tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == Tile.TileType.Empty)
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

        return WorldController.Instance.WorldData.GetTileAt(x, y);
    }
}
