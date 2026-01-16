using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Magic Combat Settings")]
    public GameObject hitEffectPrefab;  // The sparkle/explosion particle
    public float attackRange = 15f;     // Range of the spell
    public float damageAmount = 25f;    // Damage per hit
    public LayerMask enemyLayer;        // Set this to "Enemy" layer

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Left Click to Attack
        if (Input.GetMouseButtonDown(0))
        {
            PerformMagicAttack();
        }
    }

    void PerformMagicAttack()
    {
        // 1. Play Animation
        if (anim != null) anim.SetTrigger("Attack");

        // 2. Find closest enemy
        Transform target = GetClosestEnemy();

        // 3. If valid target found
        if (target != null)
        {
            // A. Rotate to face enemy immediately
            Vector3 directionToEnemy = (target.position - transform.position).normalized;
            directionToEnemy.y = 0; // Keep rotation flat
            transform.rotation = Quaternion.LookRotation(directionToEnemy);

            // B. Spawn Particle on Enemy
            if (hitEffectPrefab != null)
            {
                // Spawn slightly higher (Chest level)
                Instantiate(hitEffectPrefab, target.position + Vector3.up * 1f, Quaternion.identity);
            }

            // C. Deal Damage
            Health enemyHealth = target.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }

    Transform GetClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider potentialTarget in enemies)
        {
            float dist = Vector3.Distance(transform.position, potentialTarget.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                bestTarget = potentialTarget.transform;
            }
        }
        return bestTarget;
    }

    // Draw range circle in Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}