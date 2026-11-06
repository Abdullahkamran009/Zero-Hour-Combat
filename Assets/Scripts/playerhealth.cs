using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerhealth : MonoBehaviour
{
    public static playerhealth instance;
    private PlayerTeam playerTeam;

    public int maxHealth, currentHealth;

    public float timeUntilFinalScreen = 1f;

    public string finalScreenScene;


    private void Awake()
    {
        instance = this;
    }   
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UI.instance.healthSlider.maxValue = maxHealth;
        UI.instance.healthSlider.value = currentHealth;
        UI.instance.healthText.text = "HEALTH: " + currentHealth + "/" + maxHealth;
        playerTeam = GetComponent<PlayerTeam>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamagePlayer(int damage)
    {
        currentHealth -= damage;

        UI.instance.ShowDamage();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (playerTeam.playerTeam == Team.Red)
                GameManager.instance.AddScore(Team.Blue, 1);
            else
                GameManager.instance.AddScore(Team.Red, 1);

            if (GameManager.instance.redTeamScore >= GameManager.instance.scoreToWin ||
                GameManager.instance.blueTeamScore >= GameManager.instance.scoreToWin)
            {
                // If the game is over, show final screen
                StartCoroutine(WaitingForFinalScreen());
            }
            else
            {
                // ✅ Release player's spawn point before destroy
            if (GameManager.instance != null && GameManager.instance.currentPlayerSpawn != null)
                {
                    TeamSpawnManager.instance.ReleaseSpawnPoint(GameManager.instance.currentPlayerSpawn);
                    GameManager.instance.currentPlayerSpawn = null;
                }
                

                // Otherwise, respawn the player
                GameManager.instance.OnPlayerDeath(playerTeam.playerTeam);
                Destroy(gameObject); // remove this player object so it can be re-instantiated
            }
        }

        UI.instance.healthSlider.value = currentHealth;
        UI.instance.healthText.text = "HEALTH: " + currentHealth + "/" + maxHealth;
    }

    //health pickup 
    public void HealPlayer(int heal)
    {
        currentHealth += heal;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UI.instance.healthSlider.value = currentHealth;
        UI.instance.healthText.text = "HEALTH: " + currentHealth + "/" + maxHealth;
    }

    public IEnumerator WaitingForFinalScreen()
    {
        yield return new WaitForSeconds(timeUntilFinalScreen);

        SceneManager.LoadScene(finalScreenScene);

        Cursor.lockState = CursorLockMode.None;
    }

}
