using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    float spawnTimeMin;
    [SerializeField]
    float spawnTimeMax;
    float SpawnTime;
    [SerializeField]
    private float timer;
    [SerializeField]
    int maxEnemiesToSpawn;

    [SerializeField]
    List<GameObject> enemy;  // Changed from array to List

    [SerializeField]
    int difficultyLevel = 1;  // Difficulty level (1 = easy, 2 = medium, 3 = hard)

    private void Start()
    {
        SpawnTime = spawnTimeMin;
    }

    public void Update()
    {
        // Check for key press (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            ChangeDifficulty();
        }

        timer += Time.deltaTime;
        if (timer >= SpawnTime)
        {
            if (maxEnemiesToSpawn > 0)
            {
                maxEnemiesToSpawn -= 1;
                SpawnEnemies();
                timer = 0;
            }
            else
            {
                timer = 0;
            }
        }
    }

    private void SpawnEnemies()
    {
        if (maxEnemiesToSpawn > 0)
        {
            int randomEnemy = Random.Range(0, enemy.Count);
            Instantiate(enemy[randomEnemy], transform.position, Quaternion.identity);
            maxEnemiesToSpawn -= 1;  // Decrease maxEnemiesToSpawn to spawn fewer enemies
        }

        timer = 0;
        SpawnTime = Random.Range(spawnTimeMin, spawnTimeMax);
    }

    // Method to change difficulty
    private void ChangeDifficulty()
    {
        // Cycle through difficulty levels
        difficultyLevel++;
        if (difficultyLevel > 3)  // Loop back to easy
        {
            difficultyLevel = 1;
        }

        // Adjust maxEnemiesToSpawn and spawnTime based on difficulty
        switch (difficultyLevel)
        {
            case 1:  // Easy
                maxEnemiesToSpawn = 5;  // Fewer enemies
                spawnTimeMin = 3f;      // Longer spawn time
                spawnTimeMax = 5f;
                break;
            case 2:  // Medium
                maxEnemiesToSpawn = 10; // Moderate number of enemies
                spawnTimeMin = 2f;
                spawnTimeMax = 3.5f;
                break;
            case 3:  // Hard
                maxEnemiesToSpawn = 20; // More enemies
                spawnTimeMin = 1f;
                spawnTimeMax = 2f;
                break;
        }

        // Log the current difficulty level
        Debug.Log("Difficulty level changed to: " + difficultyLevel);
    }
}
