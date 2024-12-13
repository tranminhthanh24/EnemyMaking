using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] Transform target; // Player's position
    [SerializeField] float safeDistance = 10f; // Range for "Run" animation
    [SerializeField] float stopDistance = 1.5f; // Range for "Attack" animation
    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;
    Animator animator;

    [SerializeField] private float attackCooldown = 2f; // Cooldown between attacks
    private float lastAttackTime = -Mathf.Infinity; // Time of the last attack

    private bool isPlayerInRange = false; // Track if the player is within the red circle

    private void OnDrawGizmos()
    {
        // Visualize safeDistance and stopDistance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, safeDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing on this GameObject!");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing on this GameObject!");
        }

        // Set stopping distance for NavMeshAgent
        agent.stoppingDistance = stopDistance;
    }

    private void Update()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Switch to "Run" animation if player is within safeDistance
            if (distanceToTarget <= safeDistance && distanceToTarget > stopDistance)
            {
                isPlayerInRange = false; // Player not in attack range
                agent.SetDestination(target.position);
                animator.SetBool("Run", true);
                animator.SetBool("Idle", false);
                animator.SetBool("Attack 1", false);
            }

            // Stop moving and attack if player is within stopDistance
            else if (distanceToTarget <= stopDistance)
            {
                isPlayerInRange = true;
                agent.ResetPath();
                animator.SetBool("Run", false);
                animator.SetBool("Idle", false);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PerformRandomAttack();
                    lastAttackTime = Time.time;
                }
            }

            // Switch to "Idle" animation if player is out of range
            else
            {
                isPlayerInRange = false;
                agent.ResetPath();
                animator.SetBool("Run", false);
                animator.SetBool("Idle", true);
                animator.SetBool("Attack 1", false);
            }

            // Flip sprite based on the player's position
            FlipSprite(target.position.x);
        }
    }

    // Perform a random attack animation
    private void PerformRandomAttack()
    {
        int randomAttack = Random.Range(1, 4); // Randomly choose between 1, 2, or 3

        // Trigger the corresponding attack animation
        animator.SetTrigger($"Attack {randomAttack}");

        Debug.Log("Performing attack: Enemy_Attack_" + randomAttack);
    }

    // Flip the sprite based on the player's position
    private void FlipSprite(float targetX)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = targetX <= transform.position.x;
        }
    }
}
