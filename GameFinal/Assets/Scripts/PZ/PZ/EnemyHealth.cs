using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Death Settings")]
    public float destroyDelay = 4.0f; 
    public GameObject deathEffect;    

    private Animator anim;
    private UnityEngine.AI.NavMeshAgent agent;
    private Collider myCollider;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        myCollider = GetComponent<Collider>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        
        if (anim != null) anim.SetTrigger("GetHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null) anim.SetTrigger("Die");

        
        if (agent != null) agent.enabled = false;
        if (myCollider != null) myCollider.enabled = false;

        
        Destroy(gameObject, destroyDelay);

        
    }
}