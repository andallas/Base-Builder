using UnityEngine;


public class SoundController : MonoBehaviour
{
    private float soundCooldown = 0;

    void Start()
    {
        WorldController.Instance.WorldData.RegisterOnFurniturePlaced(OnFurniturePlaced);
        WorldController.Instance.WorldData.RegisterOnTileChanged(OnTileChanged);
    }

    void Update()
    {
        soundCooldown -= Time.deltaTime;
    }

    public void OnFurniturePlaced(Furniture furniture)
    {
        PlayClip(furniture.Type);
    }

    private void OnTileChanged(Tile tile_data)
    {
        PlayClip("Floor");
    }

    private void PlayClip(string clipName)
    {
        if (soundCooldown > 0)
        {
            return;
        }
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + clipName + "_OnCreated");
        if (clip == null)
        {
            // Using default sound clip
            clip = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");
        }

        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        soundCooldown = 0.1f;
    }
}
