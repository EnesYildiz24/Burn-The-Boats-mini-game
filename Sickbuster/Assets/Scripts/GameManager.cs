using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    public TextMeshProUGUI scoreText;
    private float score = 0f;

    private const string HighScoreKey = "HighScore";

    private string powerup = "";

    private WaveSystem wave;
    public TextMeshProUGUI waveText;
    private int currentWaveCount;

    public GameObject GameOverUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadHighScore();
        UpdateScoreText();
        wave = FindObjectOfType<WaveSystem>();
    }

    private void Update()
    {
        //Wavecounter
        CountWave();
    }

     public void Death()
    {
        GameOverUI.SetActive(true);
    }
    
    private void CountWave()
    {
        if (wave != null)
        {
            currentWaveCount = wave.currentWave;
            waveText.text = "Wave: " + currentWaveCount;
        }
    }

    public void AddPoints(float points)
    {
        score += points;
        UpdateScoreText();

        if (score > GetHighScore())
        {
            SetHighScore(score);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public float GetScore()
    {
        return score;
    }

    public float GetHighScore()
    {
        return PlayerPrefs.GetFloat(HighScoreKey, 0f);
    }

    public void SetHighScore(float newHighScore)
    {
        PlayerPrefs.SetFloat(HighScoreKey, newHighScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        float highScore = GetHighScore();
    }


    /*
        1 = Waffe Level Up
        2 = Score += 100 * Welle
        3 = Lebenspickup
        4 = Nuke
        5 = 1 min instakill
    */
    public void RandomPowerup(){
        int randomNumber = Random.Range(1,5);
        switch(randomNumber){
            case 1:
                powerup = "WEAPON";
                break;
            case 2:
                powerup = "HEALTH";
                break;
            case 3:
                powerup = "NUKE";
                break;
            case 4:
                powerup = "INSTAKILL";
                break;
        }
    }

    public string GetPowerUp(){
        return this.powerup;
    }
}
