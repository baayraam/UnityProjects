using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenü : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Setze die Spielzeit wieder auf Normal (unpausiert)
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Stoppe die Spielzeit (pausiere das Spiel)
        isPaused = true;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Start Screen");
        Time.timeScale = 1f; // Setze die Spielzeit wieder auf Normal (unpausiert)

    }
}
