using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MatchManager : MonoBehaviourPunCallbacks
{
    public static MatchManager Instance;

    public static bool IsGameOver = false;

    public float matchDuration = 300f; // 5 minutes
    public int killsToWin = 20;

    public TMP_Text timerText;
    public GameObject endScreen;
    public TMP_Text winnerText;

    private float timeRemaining;
    private bool matchEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeRemaining = matchDuration;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (matchEnded) return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(timeRemaining, 0); // Prevent negative value

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (PhotonNetwork.IsMasterClient && timeRemaining <= 0f)
        {
            EndMatchByTime();
        }
    }

    public void CheckWinCondition(Player player)
    {
        if (matchEnded) return;

        if (player.CustomProperties.TryGetValue("kills", out object value) &&
            (int)value >= killsToWin)
        {
            photonView.RPC("RPC_EndMatch", RpcTarget.All, player.NickName);
        }
    }

    void EndMatchByTime()
    {
        if (matchEnded) return; // Prevent multiple calls

        matchEnded = true;

        string winner = "No Winner";
        int topKills = -1;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int kills = player.CustomProperties.TryGetValue("kills", out object value) ? (int)value : 0;
            if (kills > topKills)
            {
                topKills = kills;
                winner = player.NickName;
            }
        }

        photonView.RPC("RPC_EndMatch", RpcTarget.All, winner);
    }


    [PunRPC]
    void RPC_EndMatch(string winnerName)
    {
        matchEnded = true;
        IsGameOver = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (endScreen != null)
            endScreen.SetActive(true);

        if (winnerText != null)
            winnerText.text = $"Winner: {winnerName}";

        GameObject playerUI = GameObject.FindGameObjectWithTag("PlayerUI");
        if (playerUI != null)
            playerUI.SetActive(false);

        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToLobby()
    {
        Time.timeScale = 1f;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MultiplayerMenu");
    }


}
