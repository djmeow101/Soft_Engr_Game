
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
        animator = GetComponent<Animator>();

        if (player == null)  
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > attackRange)
        {
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
        else if (distance <= attackRange)
        {
            StartCoroutine(Attack());
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalking", false);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        agent.ResetPath();
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.5f);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(5);
            }
        }

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    // Trigger-based detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Attack());
        }
    }
}
