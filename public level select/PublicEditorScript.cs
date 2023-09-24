using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PublicEditorScript : MonoBehaviour
{
    [SerializeField]
    Tilemap defaultTilemap; // Standard-Tilemap

    Tilemap currentTilemap
    {
        get
        {
            if (LevelManager.instance.layers.TryGetValue((int)LevelManager.instance.tiles[_selectedTileIndex].tilemap, out Tilemap tilemap))
            {
                return tilemap; // Tilemap des ausgewählten Tiles zurückgeben
            }
            else
            {
                return defaultTilemap; // Standard-Tilemap zurückgeben, wenn keine spezifische Tilemap für das ausgewählte Tile vorhanden ist
            }
        }
    }
    TileBase currentTile
    {
        get { return LevelManager.instance.tiles[_selectedTileIndex].tile; } // Aktuelles Tile zurückgeben
    }

    [SerializeField]
    bool allowTilePlacement = true; // Ermöglicht das Platzieren von Tiles

    int _selectedTileIndex; // Index des ausgewählten Tiles

    private void Start()
    {
    }

    public void IncreaseButton()
    {
        _selectedTileIndex++; // Index erhöhen
        if (_selectedTileIndex >= LevelManager.instance.tiles.Count)
        {
            _selectedTileIndex = 0; // Index auf 0 setzen, wenn er größer oder gleich der Anzahl der Tiles ist
        }
        Debug.Log(LevelManager.instance.tiles[_selectedTileIndex].name); // Namen des ausgewählten Tiles ausgeben
    }

    public void DecreaseButton()
    {
        _selectedTileIndex--; // Index verringern
        if (_selectedTileIndex < 0)
        {
            _selectedTileIndex = LevelManager.instance.tiles.Count - 1; // Index auf die letzte Position setzen, wenn er kleiner als 0 ist
        }
        Debug.Log(LevelManager.instance.tiles[_selectedTileIndex].name); // Namen des ausgewählten Tiles ausgeben
    }

    void PlaceTile(Vector3Int pos)
    {
        currentTilemap.SetTile(pos, LevelManager.instance.tiles[_selectedTileIndex].tile); // Tile an der angegebenen Position platzieren
    }

    void DeleteTile(Vector3Int pos)
    {
        currentTilemap.SetTile(pos, null); // Tile an der angegebenen Position löschen
    }

    bool IsPointerOverButton()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
            {
                return true; // Überprüfen, ob der Mauszeiger über einem Button liegt
            }
        }

        return false; // Zurückgeben, ob der Mauszeiger über einem Button liegt oder nicht
    }

    bool IsPointerOverTMPInputField()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<TMP_InputField>() != null)
            {
                return true; // Überprüfen, ob der Mauszeiger über einem TMP_InputField liegt
            }
        }

        return false; // Zurückgeben, ob der Mauszeiger über einem TMP_InputField liegt oder nicht
    }
}
