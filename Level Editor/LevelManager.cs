using System.Data.Common;
using System.Reflection.Emit;
using System.Net.Mime;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;
using UnityEngine.Networking;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // Statische Instanz des LevelManagers, um auf sie von anderen Klassen aus zugreifen zu können
    public TMP_InputField levelNameInputField; // Referenz auf das TMP_InputField im Inspector, in dem der Levelname eingegeben wird
    public TMP_InputField levelInput; // Referenz auf das TMP_InputField im Inspector, in dem der Levelcode eingegeben wird

    private string phpScriptURL = "http://localhost/Adventure Maker/DBConnection.php"; // URL zum PHP-Skript zum Speichern des Levels in der Datenbank
    private string phpScriptLoadURL = "http://localhost/Adventure Maker/LoadLevel.php"; // URL zum PHP-Skript zum Laden des Levels aus der Datenbank

    private void Awake()
    {
        // Stellt sicher, dass nur eine Instanz des LevelManagers existiert
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        
        // Fügt die Tilemaps zur layers-Dictionary hinzu
        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Tilemaps num in System.Enum.GetValues(typeof(Tilemaps)))
            {
                if (tilemap.name == num.ToString())
                {
                    if (!layers.ContainsKey((int)num))
                        layers.Add((int)num, tilemap);
                }
            }
        }
    }

    public List<CustomTile> tiles = new List<CustomTile>(); // Liste der benutzerdefinierten Tiles

    [SerializeField]
    public List<Tilemap> tilemaps = new List<Tilemap>(); // Liste der Tilemaps im Inspector
    public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>(); // Dictionary, das die Zuordnung von Layer-IDs zu Tilemaps speichert

    public enum Tilemaps // Eine Aufzählung, die verschiedene Tilemaps repräsentiert
    {
        Ground = 10,
        Obstacles = 20,
        Water = 30,
        Goal = 40
    }

    private void Update() { } // Leere Update-Methode, da keine Aktualisierung erforderlich ist

    public void Savelevel() // Methode zum Speichern des Levels
    {
        // Erstellt ein LevelData-Objekt zum Speichern der Daten
        LevelData levelData = new LevelData();

        // Fügt für jede Layer-Nummer eine neue LayerData zum LevelData-Objekt hinzu
        foreach (var item in layers.Keys)
        {
            levelData.layers.Add(new LayerData(item));
        }
        
        // Durchläuft jede Layer und speichert die Tile-Informationen
        foreach (var layerData in levelData.layers)
        {
            if (!layers.TryGetValue(layerData.layer_id, out Tilemap tilemap))
            {
                break;
            }
            
            BoundsInt bounds = tilemap.cellBounds;

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));
                    CustomTile temptile = tiles.Find(t => t.tile == temp);
                    if (temptile != null)
                    {
                        layerData.tiles.Add(temptile.id);
                        layerData.poses_x.Add(x);
                        layerData.poses_y.Add(y);
                    }
                }
            }
        }
        
        // Konvertiert das LevelData-Objekt in JSON und speichert es in einer Datei
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Application.dataPath + "/testLevel.json", json);
        string levelName = levelNameInputField.text;

        // Sendet die Daten an ein PHP-Skript
        StartCoroutine(SendDataToPHP(json, levelName));
        Debug.Log("Level Saved!");
    }

    private IEnumerator SendDataToPHP(string jsonData, string levelName)
    {
        // Daten an das PHP-Skript senden
        WWWForm form = new WWWForm();
        form.AddField("jsonData", jsonData);
        form.AddField("levelName", levelName);

        UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form);
        yield return www.SendWebRequest();

        // Überprüft den Erfolg des Requests und gibt die entsprechende Meldung aus
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Daten erfolgreich an das PHP-Skript gesendet und in die Datenbank gespeichert.");
            Debug.Log("Antwort vom Server: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Fehler beim Senden der Daten an das PHP-Skript: " + www.error);
        }
    }

    public void LoadLevel(string levelName) // Methode zum Laden eines Levels
    {
        StartCoroutine(LoadLevelFromDatabase(levelName));
    }

    public IEnumerator LoadLevelFromDatabase(string levelName)
    {
        // Daten aus der Datenbank abrufen
        WWWForm form = new WWWForm();
        form.AddField("levelName", levelName);

        UnityWebRequest www = UnityWebRequest.Post(phpScriptLoadURL, form);
        yield return www.SendWebRequest();

        // Überprüft den Erfolg des Requests und lädt das Level
        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;

            // JSON-Daten deserialisieren und das Level laden
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            foreach (var data in levelData.layers)
            {
                if (!layers.TryGetValue(data.layer_id, out Tilemap tilemap))
                {
                    break;
                }

                tilemap.ClearAllTiles();

                for (int i = 0; i < data.tiles.Count; i++)
                {
                    tilemap.SetTile(
                        new Vector3Int(data.poses_x[i], data.poses_y[i], 0),
                        tiles.Find(t => t.id == data.tiles[i]).tile
                    );
                }
            }

            Debug.Log("Level was loaded");
        }
        else
        {
            Debug.LogError("Fehler beim Laden des Levels aus der Datenbank: " + www.error);
        }
        Debug.Log("Downloaded JSON: " + www.downloadHandler.text);
    }
}

[System.Serializable]
public class LevelData
{
    public List<LayerData> layers = new List<LayerData>(); // Liste der LayerData-Objekte für jede Layer im Level
}

[System.Serializable]
public class LayerData
{
    public int layer_id; // ID der Layer
    public List<string> tiles = new List<string>(); // Liste der Tile-IDs auf dieser Layer
    public List<int> poses_x = new List<int>(); // Liste der X-Positionen der Tiles auf dieser Layer
    public List<int> poses_y = new List<int>(); // Liste der Y-Positionen der Tiles auf dieser Layer

    public LayerData(int id)
    {
        layer_id = id;
    }
}