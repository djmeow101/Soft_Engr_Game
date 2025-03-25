using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 35;
    public float attackRange = 1f;
    public LayerMask enemyLayer; // Set this to "Enemy" in the Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click to attack
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Player attacking...");

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // Draw attack range in Scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
