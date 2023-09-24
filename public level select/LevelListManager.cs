using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using TMPro;

public class LevelListManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Prefab für die Level-Buttons
    public Transform buttonContainer; // Container, in dem die Level-Buttons platziert werden

    private void Start()
    {
        StartCoroutine(GetLevelNames()); // Coroutine zum Abrufen der Levelnamen starten
    }

    private IEnumerator GetLevelNames()
    {
        string url = "http://localhost/Adventure Maker/GetLevelNames.php"; // Die URL zum GetLevelNames.php-Skript anpassen

        using (WWW www = new WWW(url))
        {
            yield return www; // Auf das Ergebnis warten

            if (string.IsNullOrEmpty(www.error))
            {
                string json = www.text; // JSON-Daten erhalten
                LevelNameList levelNameList = JsonUtility.FromJson<LevelNameList>(json); // JSON-Daten deserialisieren

                CreateLevelButtons(levelNameList.levelNames); // Level-Buttons erstellen
            }
            else
            {
                Debug.LogError("Fehler beim Abrufen der Levelnamen: " + www.error); // Bei einem Fehler eine Fehlermeldung ausgeben
            }
        }
    }

    private void CreateLevelButtons(string[] levelNames)
    {
        foreach (string levelName in levelNames)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer); // Button-GameObject aus dem Prefab erstellen und im Container platzieren
            Button button = buttonGO.GetComponent<Button>(); // Button-Komponente des GameObjects erhalten
            TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>(); // Text-Komponente des Buttons erhalten
            buttonText.text = levelName; // Text des Buttons auf den Levelnamen setzen
            button.onClick.AddListener(() => LoadLevel(levelName)); // Dem Button einen Listener hinzufügen, der das entsprechende Level lädt
        }
    }

    private void LoadLevel(string levelName)
    {
        Debug.Log("Lade Level: " + levelName); // Eine Meldung in der Konsole ausgeben, welches Level geladen wird
        LevelManager loadManager = GetComponent<LevelManager>(); // LevelManager-Komponente des GameObjects erhalten
        loadManager.LoadLevel(levelName); // Level laden
    }
}

[System.Serializable]
public class LevelNameList
{
    public string[] levelNames; // Liste der Levelnamen
}
