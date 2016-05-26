using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    private static World _worldData;
    public static World WorldData
    {
        get
        {
            if (_worldData == null)
            {
                _worldData = new World();
            }

            return _worldData;
        }
    }

    
    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found extra world controller.");
        }
        Instance = this;

        Camera.main.transform.position = new Vector3(WorldData.Width / 2, WorldData.Height / 2, Camera.main.transform.position.z);
	}


    public Tile GetTileAtWorldCoord(Vector3 vec)
    {
        int x = Mathf.FloorToInt(vec.x);
        int y = Mathf.FloorToInt(vec.y);

        return WorldData.GetTileAt(x, y);
    }
}
