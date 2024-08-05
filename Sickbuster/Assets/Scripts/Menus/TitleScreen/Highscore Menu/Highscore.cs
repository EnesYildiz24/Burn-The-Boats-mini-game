using UnityEngine;
using TMPro;

public class Highscore : MonoBehaviour
{
    public TextMeshProUGUI highscoreText;
    void Awake()
    {
        if (highscoreText != null){
            highscoreText.text = "Highscore: " + PlayerPrefs.GetFloat("HighScore", 0f);;
        }
    }
}
