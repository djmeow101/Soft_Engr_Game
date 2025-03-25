using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        animator.SetTrigger("Die"); // Play death animation
        GetComponent<Collider>().enabled = false; // Disable enemy hitbox
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false; // Stop movement
        
        // Stop the animator from transitioning after death animation starts
        StartCoroutine(DisableAnimatorAfterDeath());
        
        Destroy(gameObject, 1.5f); // Destroy after death animation
    }

    IEnumerator DisableAnimatorAfterDeath()
    {
        yield return new WaitForSeconds(0.9f); // Adjust this to match your death animation length
        animator.enabled = false; // This stops any transitions from happening
    }

}

