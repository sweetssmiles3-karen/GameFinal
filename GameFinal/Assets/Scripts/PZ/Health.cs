using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI & Events")]
    // Drag your Health Bar Slider here in the Inspector!
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDie;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f); // Set UI to full at start
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Update UI automatically without hardcoding
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        OnTakeDamage?.Invoke();

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDie?.Invoke();

        // Disable physics/movement
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        if (GetComponent<CharacterController>()) GetComponent<CharacterController>().enabled = false;

        Destroy(gameObject, 4f); // Wait for death animation
    }
}