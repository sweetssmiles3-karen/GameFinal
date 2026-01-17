using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 100f;

    // CHANGE 1: [SerializeField] makes this visible in the Inspector!
    [SerializeField] private float currentHealth;

    // CHANGE 2: Type how many seconds to wait before destroying body here
    public float destroyDelay = 4.0f;

    // Helper for Heavy Attack (Keep this so your combat script doesn't break)
    [HideInInspector] public bool isInvulnerable = false;

    [Header("UI & Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDie;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(1f); // Update UI to full
    }

    public void TakeDamage(float amount)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= amount;

        // Update the UI
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDie?.Invoke();

        // Disable Physics & Movement so the body doesn't slide away
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        if (GetComponent<UnityEngine.AI.NavMeshAgent>()) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        // Disable Player Scripts if this is a player
        if (GetComponent<PlayerMovement>()) GetComponent<PlayerMovement>().enabled = false;
        if (GetComponent<PlayerCombat>()) GetComponent<PlayerCombat>().enabled = false;

        // CHANGE 2: Uses the variable you set in the Inspector
        Destroy(gameObject, destroyDelay);
    }
}