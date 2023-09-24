using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LevelEditorScript : MonoBehaviour
{
    [SerializeField]
    Tilemap defaultTilemap; // Serialisierte Variable vom Typ `Tilemap`, die eine Referenz auf eine Standard-Tilemap speichert.

    Tilemap currentTilemap
    {
        get
        {
            // Überprüft, ob die Tilemap des ausgewählten Tiles in der `layers`-Dictionary des `LevelManager` vorhanden ist
            // und gibt sie zurück. Andernfalls wird die Standard-Tilemap verwendet.
            if (LevelManager.instance.layers.TryGetValue((int)LevelManager.instance.tiles[_selectedTileIndex].tilemap, out Tilemap tilemap))
            {
                return tilemap;
            }
            else
            {
                return defaultTilemap;
            }
        }
    }

    TileBase currentTile
    {
        get { return LevelManager.instance.tiles[_selectedTileIndex].tile; } // Gibt das aktuelle Tile basierend auf dem ausgewählten Tileindex zurück.
    }

    [SerializeField]
    Image tileImage; // Referenz auf ein Image-Objekt im Inspector, das zur Anzeige des ausgewählten Tiles verwendet wird.

    [SerializeField]
    Camera cam; // Referenz auf die Kamera im Inspector, die für die Positionierung der Tiles verwendet wird.

    [SerializeField]
    bool allowTilePlacement = true; // Eine boolsche Variable, die steuert, ob das Platzieren von Tiles erlaubt ist.

    int _selectedTileIndex; // Der Index des ausgewählten Tiles.

    private void Start()
    {
        UpdateTileImage(); // Aktualisiert das angezeigte Image für das ausgewählte Tile.
    }

    private void Update()
    {
        Vector3Int pos = currentTilemap.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition)); // Wandelt die Bildschirmposition der Maus in eine Zellenposition in der Tilemap um.

        if (
            Input.GetMouseButton(0) // Überprüft, ob die linke Maustaste gedrückt ist.
            && allowTilePlacement // Überprüft, ob das Platzieren von Tiles erlaubt ist.
            && !IsPointerOverButton() // Überprüft, ob sich der Mauszeiger über einem Button befindet.
            && !IsPointerOverTMPInputField() // Überprüft, ob sich der Mauszeiger über einem TMP_InputField befindet.
        )
        {
            PlaceTile(pos); // Platziert ein Tile an der berechneten Position.
        }
        if (Input.GetMouseButton(1)) // Überprüft, ob die rechte Maustaste gedrückt ist.
        {
            DeleteTile(pos); // Löscht ein Tile an der berechneten Position.
        }
    }

    public void IncreaseButton()
    {
        _selectedTileIndex++; // Erhöht den Index des ausgewählten Tiles.
        if (_selectedTileIndex >= LevelManager.instance.tiles.Count)
        {
            _selectedTileIndex = 0; // Setzt den Index auf 0, wenn der maximale Index überschritten wird.
        }
        UpdateTileImage(); // Aktualisiert das angezeigte Image für das ausgewählte Tile.
        Debug.Log(LevelManager.instance.tiles[_selectedTileIndex].name); // Gibt den Namen des ausgewählten Tiles in der Konsole aus.
    }

    public void DecreaseButton()
    {
        _selectedTileIndex--; // Verringert den Index des ausgewählten Tiles.
        if (_selectedTileIndex < 0)
        {
            _selectedTileIndex = LevelManager.instance.tiles.Count - 1; // Setzt den Index auf den maximalen Index, wenn er kleiner als 0 wird.
        }
        UpdateTileImage(); // Aktualisiert das angezeigte Image für das ausgewählte Tile.
        Debug.Log(LevelManager.instance.tiles[_selectedTileIndex].name); // Gibt den Namen des ausgewählten Tiles in der Konsole aus.
    }

    void UpdateTileImage()
    {
        tileImage.sprite = LevelManager.instance.tiles[_selectedTileIndex].image; // Aktualisiert das angezeigte Image für das ausgewählte Tile.
    }

    void PlaceTile(Vector3Int pos)
    {
        currentTilemap.SetTile(pos, LevelManager.instance.tiles[_selectedTileIndex].tile); // Platziert ein Tile in der Tilemap an der berechneten Position.
    }

    void DeleteTile(Vector3Int pos)
    {
        currentTilemap.SetTile(pos, null); // Löscht ein Tile in der Tilemap an der berechneten Position.
    }

    bool IsPointerOverButton()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results); // Führt einen Raycast aus, um zu überprüfen, ob sich der Mauszeiger über einem Button befindet.

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null) // Überprüft, ob der Raycast auf ein Button-Objekt getroffen ist.
            {
                return true; // Gibt zurück, dass sich der Mauszeiger über einem Button befindet.
            }
        }

        return false; // Gibt zurück, dass sich der Mauszeiger nicht über einem Button befindet.
    }

    bool IsPointerOverTMPInputField()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results); // Führt einen Raycast aus, um zu überprüfen, ob sich der Mauszeiger über einem TMP_InputField befindet.

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<TMP_InputField>() != null) // Überprüft, ob der Raycast auf ein TMP_InputField-Objekt getroffen ist.
            {
                return true; // Gibt zurück, dass sich der Mauszeiger über einem TMP_InputField befindet.
            }
        }

        return false; // Gibt zurück, dass sich der Mauszeiger nicht über einem TMP_InputField befindet.
    }
}
