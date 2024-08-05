using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public Sound[] musicSounds;
    public AudioSource musicSource;
    public Slider musicSlider;

    private void Start()
    {
        // Überprüfe, ob die Audioquellen und der Slider zugewiesen sind
        if (musicSource == null || musicSlider == null)
        {
            Debug.LogError("Music sources or slider not assigned!");
            return;
        }

        // Wenn du in der Titlescreen-Szene bist, spiele die Titlescreen-Musik und stelle die Lautstärke ein
        if (SceneManager.GetActiveScene().name == "Titlescreen")
        {
            musicSource.Play();
            musicSource.volume = musicSlider.value;
        }
        // Wenn du in der Ingame-Szene bist, spiele die Ingame-Musik und stelle die Lautstärke ein
        else if (SceneManager.GetActiveScene().name == "Ingame")
        {
            musicSource.Play();
            musicSource.volume = musicSlider.value;
        }
    }

    public void OnMusicSliderChanged()
    {
        // Aktualisiere die Lautstärke beider Audioquellen basierend auf dem Slider-Wert
        musicSource.volume = musicSlider.value;
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
}
