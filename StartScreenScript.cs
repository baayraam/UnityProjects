using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenScript : MonoBehaviour
{
    // Start is called before the first frame update

    public void Level1Click()
    {
        SceneManager.LoadScene("Level 1 - Easy");
    }
    public void Level2Click()
    {
        SceneManager.LoadScene("Level 2 - Intermediate");
    }
    public void Level3Click()
    {
        SceneManager.LoadScene("Level 3 - Hard");
    }
     public void LevelEditorClick()
    {
        SceneManager.LoadScene("Level Editor");
    }
    public void PublicLevelClick()
    {
        SceneManager.LoadScene("Public Level Select");
    }
    
    public void QuitClick()
    {
          #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
