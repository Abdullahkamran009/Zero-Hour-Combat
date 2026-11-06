using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float timeToSpawn;
    private float spawnCounter;

    public Team teamToSpawn; // Set in Inspector: Red or Blue
    public int maxBots = 3;  // Max bots per team (adjust if player is part of the team)


    // 🆕 Global lock to avoid double spawns
    private static float globalSpawnCooldown = 0f;

    void Start()
    {
        spawnCounter = timeToSpawn;
    }

    void Update()
    {
        spawnCounter -= Time.deltaTime;

        if (spawnCounter <= 0)
        {
            spawnCounter = timeToSpawn;

            // 🛑 Prevent duplicate spawns in the same frame
            if (Time.time < globalSpawnCooldown) return;

            if (GetAliveBotCount() < maxBots)
            {
                // ✅ Get a random spawn point for this team
                Transform spawnPoint = TeamSpawnManager.instance.GetRandomSpawnPoint(teamToSpawn);

                if (spawnPoint != null)
                {
                    GameObject enemyObj = Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);

                    EnemyMove enemy = enemyObj.GetComponent<EnemyMove>();
                    if (enemy != null)
                    {
                        enemy.team = teamToSpawn;
                        enemy.spawnPointUsed = spawnPoint; // 🆕 Store it for later release
                        GameManager.instance.RegisterEnemy(enemy);
                    }

                    globalSpawnCooldown = Time.time + 0.2f;
                }

            }
        }
    }

    int GetAliveBotCount()
    {
        if (teamToSpawn == Team.Red)
            return GameManager.instance.redTeam.Count;
        else
            return GameManager.instance.blueTeam.Count;
    }
}