using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public Animator animator;

    private bool isDead = false;

    private EnemyMove enemyMove;


    public void TakeDamage (float amount)
    {
        if (isDead) return; // Prevent taking damage after dead

        Debug.Log("TakeDamage() called on: " + gameObject.name);

        if (enemyMove == null) enemyMove = GetComponent<EnemyMove>(); // ✅ Get reference to EnemyMove

        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("die");
        if (GetComponent<UnityEngine.AI.NavMeshAgent>())
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        if (enemyMove != null)
        {
            if (enemyMove.team == Team.Red)
                GameManager.instance.redTeam.Remove(enemyMove);
            else
                GameManager.instance.blueTeam.Remove(enemyMove);

            if (enemyMove.team == Team.Red)
                GameManager.instance.AddScore(Team.Blue, 1); // Blue scored
            else
                GameManager.instance.AddScore(Team.Red, 1);  // Red scored

            // ✅ Release the used spawn point
            if (enemyMove.spawnPointUsed != null)
                TeamSpawnManager.instance.ReleaseSpawnPoint(enemyMove.spawnPointUsed);
        }

        StartCoroutine(DeathCoroutine());
    }

    System.Collections.IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2f); // Wait death animation time (adjust to match your animation)

        UI.instance.TotalKills++;
        UI.instance.killedEnemiesText.text = "Total Kills: " + UI.instance.TotalKills;

        Destroy(gameObject); // ✅ Finally destroy the enemy
    }

}
