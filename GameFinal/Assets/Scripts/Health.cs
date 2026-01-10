using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Optional: Play Hurt Animation
        // if(anim != null) anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (anim != null) anim.SetTrigger("Die");

        // Disable physics and logic
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;

        // Stop Movement scripts if they exist
        if (GetComponent<UnityEngine.AI.NavMeshAgent>()) GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        if (GetComponent<PlayerController>()) GetComponent<PlayerController>().enabled = false;
        if (GetComponent<PlayerAttack>()) GetComponent<PlayerAttack>().enabled = false;
        if (GetComponent<EnemyAI>()) GetComponent<EnemyAI>().enabled = false;

        Destroy(gameObject, 4f);
    }
}