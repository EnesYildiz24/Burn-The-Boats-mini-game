using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SFXController : MonoBehaviour
{
    public Sound[] sfxSounds;
    public AudioSource sfxSource;
    public Slider sfxSlider;

    private void Start()
    {
        if (sfxSource == null || sfxSlider == null)
        {
            Debug.LogError("SFX sources or slider not assigned!");
            return;
        }

        if (SceneManager.GetActiveScene().name == "Title Screen")
        {
            sfxSource.Play();
            sfxSource.volume = sfxSlider.value;
        }
        else if (SceneManager.GetActiveScene().name == "Ingame Screen")
        {
            sfxSource.Play();
            sfxSource.volume = sfxSlider.value;
        }
    }

    public void OnSFXSliderChanged()
    {
        sfxSource.volume = sfxSlider.value;
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
