using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScreen : MonoBehaviour
{

    public string mainMenuScene;

    public TextMeshProUGUI killedEnemiesText;
    public TextMeshProUGUI resultText;

    public static FinalScreen instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        string winner = PlayerPrefs.GetString("MatchWinner", "None");
        string playerTeam = PlayerPrefs.GetString("PlayerTeam", "None");
        int totalKills = PlayerPrefs.GetInt("KilledEnemies", -1);

        if (winner == "Draw")
        {
            resultText.text = "DRAW!";
        }
        else if (winner == playerTeam)
        {
            resultText.text = "You Won!";
        }
        else
        {
            resultText.text = "You Lose!!";
        }

        killedEnemiesText.text = "TOTAL KILLS: " + totalKills;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
