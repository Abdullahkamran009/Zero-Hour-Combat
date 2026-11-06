using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public static UI instance;
    private bool isLocalPlayer = false;

    public Slider healthSlider;
    public TextMeshProUGUI healthText, ammoText, killedEnemiesText;

    public GameObject pauseScreen;

    public Image damageEffect;

    public float damageAlpha = 0.3f, damageFadeSpeed = 3f;

    public int TotalKills;

    public TextMeshProUGUI matchTimerText;
    public TextMeshProUGUI redScoreText;
    public TextMeshProUGUI blueScoreText;


    public void UpdateAmmo(int current, int max, int reserve)
    {
        ammoText.text = current + " / " + max + "   |   " + reserve;
    }

    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (damageEffect.color.a != 0)
        {
            damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, Mathf.MoveTowards(damageEffect.color.a, 0f, damageFadeSpeed * Time.deltaTime));
        }
    }


    public void ShowDamage()
    {
        damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, 0.3f);
    }

    public void UpdateMatchTimer(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        matchTimerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void UpdateTeamScores(int red, int blue)
    {
        redScoreText.text = $"{red}";
        blueScoreText.text = $"{blue}";
    }

}
