using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;
   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
       
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
           Cursor.lockState = CursorLockMode.None; 
           Cursor.visible = true; 
           Time.timeScale = 0f;
            }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
       #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
