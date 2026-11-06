using System.Collections.Generic;
using UnityEngine;

public class TeamSpawnManager : MonoBehaviour
{
    public static TeamSpawnManager instance;

    public List<Transform> redBotSpawns = new();
    public List<Transform> blueBotSpawns = new();
    public List<Transform> redPlayerSpawns = new();
    public List<Transform> bluePlayerSpawns = new();


    private HashSet<Transform> usedSpawns = new(); // 🆕 Track used points

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Transform GetRandomSpawnPoint(Team team, bool isPlayer = false)
    {
        List<Transform> pool = null;

        if (team == Team.Red)
            pool = isPlayer ? redPlayerSpawns : redBotSpawns;
        else
            pool = isPlayer ? bluePlayerSpawns : blueBotSpawns;

        List<Transform> available = pool.FindAll(sp => !usedSpawns.Contains(sp));

        if (available.Count > 0)
        {
            Transform chosen = available[Random.Range(0, available.Count)];
            usedSpawns.Add(chosen);
            return chosen;
        }

        return null;
    }


    public void ReleaseSpawnPoint(Transform spawn)
    {
        if (usedSpawns.Contains(spawn))
            usedSpawns.Remove(spawn); // Free up on death
    }
}

