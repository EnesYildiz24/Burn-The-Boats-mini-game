using UnityEngine;
using System.Collections.Generic;

public class WaveSystem : MonoBehaviour
{
    public Enemy bacteria;
    public Enemy virus;
    public Enemy cancer;
    public int startBacteriaCountInWave = 30;
    private int bacteriaCountInWave;
    public int currentWave = 0;
    private bool isWaveActive = false;

    void Start()
    {
        StartNextWave();
    }

    void StartNextWave()
    {
        currentWave++;
        isWaveActive = true;

        bacteriaCountInWave = CalculateBacteriaCount(currentWave);
        SpawnBacteria(bacteriaCountInWave);
    }

    int CalculateBacteriaCount(int waveNumber)
    {
        return startBacteriaCountInWave * waveNumber;
    }

    void SpawnBacteria(int count)
    {
        if (currentWave % 2 == 0)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(bacteria, GetRandomSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < currentWave * 5 / 2; i++)
            {
                Instantiate(virus, GetRandomSpawnPosition(), Quaternion.identity);
            }
            if (currentWave % 4 == 0)
            {
                for (int i = 0; i < currentWave * 5 / 4; i++)
                {
                    Instantiate(cancer, GetRandomSpawnPosition(), Quaternion.identity);
                }
            }
        }
        else
        {  
            for (int i = 0; i < count; i++)
            {
                Instantiate(bacteria, GetRandomSpawnPosition(), Quaternion.identity);
            }
        }

    }

    Vector3 GetRandomSpawnPosition()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float bufferDistance = 30f; 

        int side = Random.Range(0, 4);

        float randomX = 0.0f;
        float randomY = 0.0f;

        switch (side)
        {
            case 0: 
                randomX = Random.Range(-screenWidth - bufferDistance, -bufferDistance);
                randomY = Random.Range(-screenHeight, screenHeight);
                break;

            case 1:
                randomX = Random.Range(screenWidth + bufferDistance, screenWidth + bufferDistance * 2);
                randomY = Random.Range(-screenHeight, screenHeight);
                break;

            case 2:
                randomX = Random.Range(-screenWidth, screenWidth);
                randomY = Random.Range(screenHeight + bufferDistance, screenHeight + bufferDistance * 2);
                break;

            case 3:
                randomX = Random.Range(-screenWidth, screenWidth);
                randomY = Random.Range(-screenHeight - bufferDistance, -bufferDistance);
                break;
        }

        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(randomX, randomY, 0.0f));

        spawnPosition.z = 0.0f;

        return spawnPosition;
    }


    void Update()
    {
        if (isWaveActive)
        {
            if (AreAllBacteriaDefeated())
            {
                isWaveActive = false;
                StartNextWave();
            }
        }
    }

    bool AreAllBacteriaDefeated()
    {
        Enemy[] activeBacteria = FindObjectsOfType<Enemy>();
        return activeBacteria.Length == 0;
    }
}
