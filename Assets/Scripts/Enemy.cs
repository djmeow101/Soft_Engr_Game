using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;



public class EnemyAI : MonoBehaviour
{
    public Transform player;  
    private NavMeshAgent agent;
    public Animator animator;

    public float detectionRange = 10f;
    public float attackRange = 2f;

    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(new Vector3(0, 0, 0));
        animator = GetComponent<Animator>();
        

        if (player == null)  
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            
            if (player == null)
            {
                Debug.LogError("Player not found! Make sure the Knight GameObject has the 'Player' tag.");
            }
            else
            {
                Debug.Log("Player assigned: " + player.name);
            }
        }
    }


    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        

        if (distance <= detectionRange && distance > attackRange)
        {
            Debug.Log("Enemy moving toward player...");  // <-- Debugging movement
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
            isAttacking = false;
        }
        else if (distance <= attackRange && !isAttacking)
        {
            Debug.Log("Enemy attacking player!");  // <-- Debugging attack phase
            StartCoroutine(Attack());
        }
        else
        {
            
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }


    IEnumerator Attack()
    {
    isAttacking = true;
    agent.isStopped = true;
    animator.SetBool("isAttacking", true);

    Debug.Log("Enemy started attacking!");

    // Apply damage to the player
    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
    if (playerHealth != null)
    {
        playerHealth.TakeDamage(5); // Adjust damage value as needed
    }

    yield return new WaitForSeconds(1f);

    animator.SetBool("isAttacking", false);
    agent.isStopped = false;
    isAttacking = false;

    Debug.Log("Enemy finished attacking!");
    }

}
