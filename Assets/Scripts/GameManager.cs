using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum Team
{
    Red,
    Blue
}


public class GameManager : MonoBehaviour
{
    public float matchDuration = 300f; // 5 minutes
    private float remainingTime;
    public float timeUntilFinalScreen = 2f;
    public string finalScreenScene;
    private bool gameOver = false;
    public Transform currentPlayerSpawn;

    public GameObject playerPrefab;
    private GameObject currentPlayer;

    public static GameManager instance;

    public float waitAfterDeath = 3f;

    public int killedEnemies;

    public List<EnemyMove> redTeam = new List<EnemyMove>();
    public List<EnemyMove> blueTeam = new List<EnemyMove>();

    public int redTeamScore = 0;
    public int blueTeamScore = 0;
    public int scoreToWin = 25;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // 🟡 Player Spawn Logic
        SpawnPlayer(Team.Red);

        remainingTime = matchDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {

            PauseUnpause();
        }

        if (!gameOver)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                EndMatchByTimeout();
            }

            UI.instance.UpdateMatchTimer(remainingTime);
        }

    }

    /* public void PlayerDeath()
     {
         StartCoroutine(PlayerDeathCoroutine());
     }

     public IEnumerator PlayerDeathCoroutine()
     {
         yield return new WaitForSeconds(waitAfterDeath);

         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
     } */

    public void PauseUnpause()
    {

        Debug.Log("PauseUnpause called from: " + gameObject.name);

        if (UI.instance != null)
            Debug.Log("UI.instance: " + UI.instance.name);

        if (UI.instance.pauseScreen != null)
            Debug.Log("UI.instance.pauseScreen: " + UI.instance.pauseScreen.name);


        if (UI.instance.pauseScreen.activeInHierarchy)
        {
            UI.instance.pauseScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
        else
        {
            UI.instance.pauseScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }

    }

    public void RegisterEnemy(EnemyMove enemy)
    {
        if (enemy.team == Team.Red)
            redTeam.Add(enemy);
        else
            blueTeam.Add(enemy);
    }

    public void AddScore(Team team, int amount)
    {
        if (team == Team.Red)
            redTeamScore += amount;
        else
            blueTeamScore += amount;

        UI.instance.UpdateTeamScores(redTeamScore, blueTeamScore);

        CheckVictory();
    }

    private void CheckVictory()
    {
        if (gameOver) return;

        if (redTeamScore >= scoreToWin)
        {
            Debug.Log("🔴 Red Team Wins!");
            gameOver = true;
            StartCoroutine(LoadFinalScene("Red"));
        }
        else if (blueTeamScore >= scoreToWin)
        {
            Debug.Log("🔵 Blue Team Wins!");
            gameOver = true;
            StartCoroutine(LoadFinalScene("Blue"));
        }
    }


    public void SpawnPlayer(Team playerTeam)
    {
        Transform spawnPoint = TeamSpawnManager.instance.GetRandomSpawnPoint(playerTeam, true); // true = isPlayer
        currentPlayerSpawn = spawnPoint;

        if (spawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            FindObjectOfType<MinimapFollow>()?.SetTarget(currentPlayer.transform);

            // Make sure it has the correct team assigned
            PlayerTeam pt = currentPlayer.GetComponent<PlayerTeam>();
            if (pt != null)
                pt.playerTeam = playerTeam;

            // Smooth spawn for CharacterController
            CharacterController cc = currentPlayer.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                currentPlayer.transform.position = spawnPoint.position;
                currentPlayer.transform.rotation = spawnPoint.rotation;
                cc.enabled = true;
            }

            Debug.Log("✅ Player spawned at: " + spawnPoint.name);
        }
        else
        {
            Debug.LogWarning("⚠️ No available spawn point for team: " + playerTeam);
        }
    }

    public void OnPlayerDeath(Team team)
    {
        StartCoroutine(RespawnPlayer(team));
    }

    private IEnumerator RespawnPlayer(Team team)
    {
        yield return new WaitForSeconds(waitAfterDeath);
        SpawnPlayer(team);
    }

    private IEnumerator LoadFinalScene(string winningTeam)
    {
        Debug.Log("🏁 Game Over - " + winningTeam + " Team Wins");

        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        // Debug before wait
        Debug.Log("📝 Kills before wait: " + killedEnemies);

        yield return new WaitForSecondsRealtime(timeUntilFinalScreen);

        Debug.Log("📝 Saving kills to PlayerPrefs: " + killedEnemies);
        PlayerPrefs.SetInt("KilledEnemies", killedEnemies);
        PlayerPrefs.SetString("MatchWinner", winningTeam);
        PlayerPrefs.SetString("PlayerTeam", currentPlayer.GetComponent<PlayerTeam>().playerTeam.ToString());

        SceneManager.LoadScene(finalScreenScene);
    }


    void EndMatchByTimeout()
    {
        gameOver = true;

        string winner = redTeamScore > blueTeamScore ? "Red" :
                        blueTeamScore > redTeamScore ? "Blue" :
                        "Draw";

        Debug.Log("⏱️ Time Up! Winner: " + winner);
        StartCoroutine(LoadFinalScene(winner));
    }


}
