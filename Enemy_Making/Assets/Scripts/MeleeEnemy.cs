﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private Transform target; // Player's position
    [SerializeField] private float safeDistance = 10f; // Range for "Run" animation
    [SerializeField] private float stopDistance = 1.5f; // Range for "Attack" animation
    [SerializeField] private float attackCooldown = 2f; // Cooldown between attacks

    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float lastAttackTime = -Mathf.Infinity; // Time of the last attack
    private bool isPlayerInRange = false; // Track if the player is within attack range

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

            // Handle movement and animations based on the player's distance
            if (distanceToTarget <= safeDistance && distanceToTarget > stopDistance)
            {
                // Player within safeDistance but not stopDistance
                isPlayerInRange = false;
                agent.SetDestination(target.position);
                SetAnimationState(run: true, idle: false);
            }
            else if (distanceToTarget <= stopDistance)
            {
                // Player within attack range
                isPlayerInRange = true;
                agent.ResetPath();
                SetAnimationState(run: false, idle: false);

                // Attack if cooldown has elapsed
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PerformRandomAttack();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Player out of range
                isPlayerInRange = false;
                agent.ResetPath();
                SetAnimationState(run: false, idle: true);
            }

            // Flip sprite based on player's position
            FlipSprite(target.position.x);
        }
    }

    // Perform a random attack animation
    private void PerformRandomAttack()
    {
        int randomAttack = Random.Range(1, 4); // Randomly choose between 1, 2, or 3
        animator.SetTrigger($"Attack {randomAttack}");
        Debug.Log($"Performing attack: Attack_{randomAttack}");
    }

    // Flip the sprite based on the player's position
    private void FlipSprite(float targetX)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = targetX <= transform.position.x;
        }
    }

    // Set animation state for "Run" and "Idle"
    private void SetAnimationState(bool run, bool idle)
    {
        if (animator != null)
        {
            animator.SetBool("Run", run);
            animator.SetBool("Idle", idle);
        }
    }

    // Animation event handler for "GolemEndAbility"
    public void GolemEndAbility()
    {
        // Logic to handle the end of an attack or ability animation
        Debug.Log("Animation event: GolemEndAbility triggered.");
    }
}
