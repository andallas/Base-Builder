﻿using UnityEngine;
using System.Collections.Generic;


public class CharacterSpriteController : MonoBehaviour
{
    public Dictionary<string, Sprite> characterSprites;

    private Dictionary<Character, GameObject> characterGameObjectMap;


    void Start()
    {
        LoadSprites();

        characterGameObjectMap = new Dictionary<Character, GameObject>();

        WorldController.WorldData.RegisterOnCharacterCreated(OnCharacterCreated);

        WorldController.WorldData.CreateCharacter(WorldController.WorldData.GetTileAt(WorldController.WorldData.Width / 2, WorldController.WorldData.Height / 2));
    }


    public void OnCharacterCreated(Character character)
    {
        int x = character.CurrentTile.X;
        int y = character.CurrentTile.Y;
        GameObject character_go = new GameObject();
        character_go.name = "Character";
        character_go.transform.position = new Vector3(x, y, 0);
        character_go.transform.SetParent(transform, true);

        SpriteRenderer character_sr = character_go.AddComponent<SpriteRenderer>();
        character_sr.sprite = characterSprites["p1_front"];
        character_sr.sortingLayerName = "Characters";

        characterGameObjectMap[character] = character_go;

        //character.RegisterOnChangedCallback(OnCharacterChanged);
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

    //private void OnCharacterChanged(Character character)
    //{
    //    // TODO: Make sure the furnitures graphics have been updated
    //    if (!characterGameObjectMap.ContainsKey(character))
    //    {
    //        Debug.LogError("OnCharacterChanged - trying to change visuals for character, not found in map.");
    //        return;
    //    }
    //    GameObject character_go = characterGameObjectMap[character];
    //    SpriteRenderer spriteRenderer = character_go.GetComponent<SpriteRenderer>();
    //    if (spriteRenderer == null)
    //    {
    //        spriteRenderer = character_go.AddComponent<SpriteRenderer>();
    //    }
    //    spriteRenderer.sprite = GetSpriteForFurniture(character);
    //}
}
