using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class TileSpriteController : MonoBehaviour
{
    // TODO: Use a key/value pair of sprites, keyed on Tile.TileType
    public Sprite floorSprite;
    public Sprite emptySprite;

    private Dictionary<Tile, GameObject> tileGameObjectMap;


    void Start()
    {
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        World world = WorldController.WorldData;
        world.RegisterOnTileChanged(OnTileChanged);

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTileAt(x, y);

                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);
                tile_go.AddComponent<SpriteRenderer>().sprite = emptySprite;

                tileGameObjectMap[tile_data] = tile_go;
            }
        }
    }


    private void OnTileChanged(Tile tile_data)
    {
        if (!tileGameObjectMap.ContainsKey(tile_data))
        {
            Debug.LogError("OnTileChanged - tile_data not found!");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if (tile_go == null)
        {
            Debug.LogError("OnTileChanged - tile_go is null!");
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
            Debug.LogError("OnTileChanged - Unrecognized tile type.");
        }
    }

    //private void DestroyAllTileGameObjects()
    //{
    //    while (tileGameObjectMap.Count > 0)
    //    {
    //        Tile tile_data = tileGameObjectMap.Keys.First();
    //        GameObject tile_go = tileGameObjectMap[tile_data];

    //        tileGameObjectMap.Remove(tile_data);
    //        tile_data.UnregisterOnTileChangedCallback(OnTileChanged);
    //        Destroy(tile_go);
    //    }
    //}
}
