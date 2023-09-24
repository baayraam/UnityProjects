using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new Customtile", menuName = "LevelEditor/Tile")]
public class CustomTile : ScriptableObject
{
    public Sprite image;
    public TileBase tile;
    public string id;
    public LevelManager.Tilemaps tilemap;
}
