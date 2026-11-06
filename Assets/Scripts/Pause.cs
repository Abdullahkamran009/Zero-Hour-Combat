using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public string mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.PauseUnpause();  // Works for single player
        }
        else
        {
            // Multiplayer: fallback toggle
            if (UI.instance != null && UI.instance.pauseScreen != null)
            {
                UI.instance.pauseScreen.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Debug.LogWarning("UI.instance or pauseScreen is missing!");
            }
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
