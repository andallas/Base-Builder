using System.Collections.Generic;
using UnityEngine;


public class CharacterSpriteController : MonoBehaviour
{
    public Dictionary<string, Sprite> characterSprites;

    private Dictionary<Character, GameObject> characterGameObjectMap;


    void Start()
    {
        LoadSprites();

        characterGameObjectMap = new Dictionary<Character, GameObject>();

        World world = WorldController.WorldData;
        world.RegisterOnCharacterCreated(OnCharacterCreated);

        foreach (Character character in world.Characters)
        {
            OnCharacterCreated(character);
        }
    }


    public void OnCharacterCreated(Character character)
    {
        GameObject character_go = new GameObject();
        character_go.name = "Character";
        character_go.transform.position = new Vector3(character.X, character.Y, 0);
        character_go.transform.SetParent(transform, true);

        SpriteRenderer character_sr = character_go.AddComponent<SpriteRenderer>();
        character_sr.sprite = characterSprites["p1_front"];
        character_sr.sortingLayerName = "Characters";

        characterGameObjectMap[character] = character_go;

        character.RegisterOnChangedCallback(OnCharacterChanged);
    }


    private void LoadSprites()
    {
        characterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/");

        foreach (Sprite s in sprites)
        {
            characterSprites[s.name] = s;
        }
    }

    private void OnCharacterChanged(Character character)
    {
        // TODO: Make sure the OnCharacterChanged graphics have been updated
        if (!characterGameObjectMap.ContainsKey(character))
        {
            Debug.LogError("OnCharacterChanged - trying to change visuals for character, not found in map.");
            return;
        }
        GameObject character_go = characterGameObjectMap[character];
        character_go.transform.position = new Vector3(character.X, character.Y, 0);
    }
}
