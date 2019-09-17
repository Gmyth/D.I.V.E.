﻿using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleTile : Tile
{
    [Space(20)]
    [Header("Destructible Tile")]

    public float life;
    private float StartLife;

    /// <summary>
    /// Sprite to display when life is below 50%
    /// </summary>
    public Sprite brokenSprite;

    public ITilemap tileMap;
    public Vector3Int tilePosition;

    //Tile Overriding
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        StartLife = life;

        //Store some data
        this.tileMap = tilemap;
        this.tilePosition = position;

        return base.StartUp(position, tilemap, go);
    }

    /// <summary>
    /// Apply damage to the tile
    /// </summary>
    /// <param name="pDamage"></param>
    public void ApplyDamage(float pDamage)
    {
        life -= pDamage;
        if (life < StartLife / 2 && base.sprite != brokenSprite)
        {
            base.sprite = brokenSprite;
        }
        if (life < 0)
        {
            base.sprite = null;
        }
    }

    [MenuItem("Assets/Tools/DestructibleTile")]
    public static void CreateDestructibleTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Destructible Tile", "DestructibleTile_", "Asset", "Save Destructible Tile", "Assets");
        
        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DestructibleTile>(), path);
    }
}