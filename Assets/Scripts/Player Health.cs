using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Max Health of player
    private int currentHealth; // Current Health of player, not to exceed max health

    void Start()
    {
        currentHealth = maxHealth; 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

void Die()
{
    Debug.Log("Player has died! Returning to Start Screen...");
    SceneManager.LoadScene("Start_Screen"); // Return to start screen
}

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
