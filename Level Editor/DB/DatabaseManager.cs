using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class DatabaseManager : MonoBehaviour
{
    public TMP_InputField jsonDataInputField;
    public TMP_InputField levelNameInputField;
    public Button saveButton;

    private string phpScriptURL = "http://localhost/Adventure Maker/DBConnection.php"; // URL zum PHP-Skript

    private void Start()
    {
        saveButton.onClick.AddListener(SaveDataToDatabase);
    }

    private void SaveDataToDatabase()
    {
        string jsonData = jsonDataInputField.text;
        string levelName = levelNameInputField.text;

        StartCoroutine(SendDataToPHP(jsonData, levelName));
    }

    private IEnumerator SendDataToPHP(string jsonData, string levelName)
    {
        WWWForm form = new WWWForm();
        form.AddField("jsonData", jsonData);
        form.AddField("levelName", levelName);

        UnityWebRequest www = UnityWebRequest.Post(phpScriptURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Daten erfolgreich an das PHP-Skript gesendet.");
            Debug.Log("Antwort vom Server: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Fehler beim Senden der Daten an das PHP-Skript: " + www.error);
        }
    }
}
