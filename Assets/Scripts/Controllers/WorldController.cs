using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;


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
				_worldData = new World(100, 100);
			}

			return _worldData;
		}
	}

	private static bool loadWorld = false;

	
	void OnEnable()
	{
		if (Instance != null)
		{
			Debug.LogWarning("Found extra world controller.");
		}
		Instance = this;

		if (loadWorld)
		{
			loadWorld = false;
			CreateWorldFromSaveFile();
		}
		else
		{
			CreateEmptyWorld();
		}
	}

	void Update()
	{
		// TODO: Add pause/unpause, speed controls, etc...
		_worldData.Update(Time.deltaTime);
	}


	public Tile GetTileAtWorldCoord(Vector3 vec)
	{
		int x = Mathf.FloorToInt(vec.x);
		int y = Mathf.FloorToInt(vec.y);

		return WorldData.GetTileAt(x, y);
	}

	public void NewWorld()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void SaveWorld()
	{
		XmlSerializer worldSerializer = new XmlSerializer(typeof(World));
		TextWriter writer = new StringWriter();
		worldSerializer.Serialize(writer, _worldData);
		writer.Close();

		Debug.Log(writer.ToString());

		PlayerPrefs.SetString("SaveGame00", writer.ToString());
	}

	public void LoadWorld()
	{
		loadWorld = true;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


	private void CreateEmptyWorld()
	{
		_worldData = new World(100, 100);

		Camera.main.transform.position = new Vector3(WorldData.Width / 2, WorldData.Height / 2, Camera.main.transform.position.z);
	}

	private void CreateWorldFromSaveFile()
	{
		XmlSerializer worldSerializer = new XmlSerializer(typeof(World));
		TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
		_worldData = (World)worldSerializer.Deserialize(reader);
		reader.Close();


		Camera.main.transform.position = new Vector3(WorldData.Width / 2, WorldData.Height / 2, Camera.main.transform.position.z);
	}
}
