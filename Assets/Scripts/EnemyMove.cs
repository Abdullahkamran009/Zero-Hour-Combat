using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{

    public float speed, distancetostop;
    public Rigidbody theRgidBody;

    private Vector3 Target;

    public NavMeshAgent agent;
    public float fireRate = 1f;
    public float damage = 10f;
    public float shootingRange = 30f;
    public Transform firePoint;        // Rotating part of the gun
    public ParticleSystem muzzleflash;
    public Animator animator;
    bool isDead = false;

    public Team team;
    public Transform spawnPointUsed; // 🆕 store spawn location used


    [Header("Audio")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    private float nextTimeToFire = 0f;
    private bool hasFired = false;

    private EnemyMinimapReveal minimapReveal;

    private Transform currentTarget;
    private float targetScanInterval = 1f;
    private float targetScanTimer = 0f;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        minimapReveal = GetComponent<EnemyMinimapReveal>();
    }


    void Update()
    {
        if (isDead) return; // 🚨 skip all if dead

        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return; // 🚨 check agent

        targetScanTimer -= Time.deltaTime;
        if (targetScanTimer <= 0f)
        {
            ScanForEnemies();
            targetScanTimer = targetScanInterval;
        }

        // Follow player
        if (currentTarget == null) return;

        Target = currentTarget.position;
        Target.y = transform.position.y;
        agent.destination = Target;

        Debug.DrawRay(firePoint.position, firePoint.forward * 5f, Color.green);

        if (Vector3.Distance(transform.position, Target) > distancetostop)
        {
            agent.destination = Target;
        }
        else
        {
            agent.destination = transform.position;
        }

        // Handle movement animation
        if (agent.remainingDistance < 0.3f)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }


        // Always aim the gun at the player
        AimGunAtPlayer();

        // Shoot if in range and time has passed
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance <= shootingRange && Time.time >= nextTimeToFire)
        {
            Vector3 toTarget = (currentTarget.position + Vector3.up * 0.75f) - firePoint.position;
            float angleToTarget = Vector3.Angle(firePoint.forward, toTarget.normalized);

            // ✅ Allow shooting if close or aimed well
            if (distance < 3f || angleToTarget <= 30f)
            {
                ShootAtTarget();
            }
        }
    }


    void AimGunAtPlayer()
    {
        if (currentTarget == null) return;

        Vector3 targetPos = currentTarget.position + Vector3.up * 0.75f; // aim at chest/head
        Vector3 aimDir = targetPos - firePoint.position;

        if (aimDir != Vector3.zero)
        {
            firePoint.rotation = Quaternion.LookRotation(aimDir); // Instant accurate aim
        }
    }

    void ShootAtTarget()
    {


        if (currentTarget == null) return;

        Vector3 targetPoint = currentTarget.position + Vector3.up * 0.5f; // Aim at stomach/chest instead of head                                                                          // Aim at chest level
        Vector3 directionToTarget = (targetPoint - firePoint.position).normalized;


        float distToTarget = Vector3.Distance(transform.position, currentTarget.position);
        float angleToTarget = Vector3.Angle(firePoint.forward, directionToTarget);

        if (distToTarget < 3f)
        {
            angleToTarget = 0f;
        }

        if (angleToTarget > 30f) return;

        if (!hasFired)
        {
            animator.SetTrigger("fire");
            hasFired = true;
        }

        //reveal on minimap
        if (minimapReveal != null)
            minimapReveal.RevealOnMinimap();

        Debug.Log("Enemy fired a shot at: " + Time.time);

        if (muzzleflash != null)
            muzzleflash.Play();

        // 🔊 Play shoot sound
        if (shootSound != null && audioSource != null)
            audioSource.PlayOneShot(shootSound);

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, directionToTarget, out hit, shootingRange))
        {
            Debug.Log("Hit something: " + hit.collider.name);
            Debug.DrawRay(firePoint.position, directionToTarget * shootingRange, Color.red, 1f);

            // Hit player
            if (hit.collider.CompareTag("Player"))
            {
                playerhealth ph = hit.collider.GetComponent<playerhealth>();
                if (ph != null)
                    ph.DamagePlayer((int)damage);
            }
            else
            {
                // Hit enemy bot
                Target targetBot = hit.collider.GetComponentInParent<Target>();
                EnemyMove targetMove = hit.collider.GetComponentInParent<EnemyMove>();

                Debug.Log("targetBot: " + targetBot);
                Debug.Log("targetMove: " + targetMove);
                if (targetMove != null)
                    Debug.Log("targetMove.team = " + targetMove.team + ", this.team = " + this.team);

                if (targetBot != null && targetMove != null && targetMove.team != this.team)
                {
                    Debug.Log("✅ Enemy bot hit and taking damage: " + targetMove.name);
                    targetBot.TakeDamage(damage);
                }
            }
        }

        nextTimeToFire = Time.time + 1f / fireRate;
        hasFired = false; // reset after shooting
    }

    void ScanForEnemies()
    {
        currentTarget = null;
        float closestDistance = Mathf.Infinity;

        // 1. Scan opposing bots
        List<EnemyMove> enemyBots = team == Team.Red ? GameManager.instance.blueTeam : GameManager.instance.redTeam;
        foreach (var bot in enemyBots)
        {
            if (bot == null || bot == this) continue;
            float dist = Vector3.Distance(transform.position, bot.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                currentTarget = bot.transform;
            }
        }

        // 2. Also check player if on enemy team
        var player = FindObjectOfType<PlayerTeam>();
        if (player != null && player.playerTeam != this.team)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                currentTarget = player.transform;
            }
        }
    }


}
