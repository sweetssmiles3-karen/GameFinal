using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ranges")]
    public float meleeRange = 3.0f;     // Distance for "Smacking"
    public float magicRange = 15.0f;    // Distance for "Shooting"

    [Header("Damage")]
    public float meleeDamage = 40f;     // Hits harder because it's risky
    public float magicDamage = 20f;     // Weaker but safer

    [Header("Visuals")]
    public GameObject meleeHitEffect;   // Blood/Impact effect
    public GameObject magicHitEffect;   // Sparkles/Explosion effect
    public Transform attackPoint;       // Center of player or wand tip

    [Header("Settings")]
    public LayerMask enemyLayer;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Left Click
        if (Input.GetMouseButtonDown(0))
        {
            AdaptiveAttack();
        }
    }

    void AdaptiveAttack()
    {
        // 1. Find the closest enemy
        Transform target = GetClosestEnemy();

        // 2. If no enemy is around, just play a generic swing and exit
        if (target == null)
        {
            anim.SetTrigger("Attack"); // Play swing animation
            return;
        }

        // 3. Rotate to face them instantly
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        // 4. CHECK DISTANCE: Melee or Magic?
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= meleeRange)
        {
            // === MELEE MODE ===
            Debug.Log("Performed MELEE Attack");
            anim.SetTrigger("Attack"); // Or "MeleeAttack" if you have a specific one

            // Spawn Melee Effect (e.g., Blood)
            if (meleeHitEffect)
                Instantiate(meleeHitEffect, target.position + Vector3.up, Quaternion.identity);

            // Deal High Damage
            DealDamage(target, meleeDamage);
        }
        else if (distance <= magicRange)
        {
            // === MAGIC MODE ===
            Debug.Log("Performed MAGIC Attack");
            anim.SetTrigger("Attack"); // Or "CastAttack"

            // Spawn Magic Effect (e.g., Sparkles) on target
            if (magicHitEffect)
                Instantiate(magicHitEffect, target.position + Vector3.up, Quaternion.identity);

            // Deal Magic Damage
            DealDamage(target, magicDamage);
        }
    }

    void DealDamage(Transform enemy, float amount)
    {
        // Uses the Interface "High Tech" method
        IDamageable damageable = enemy.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(amount);
        }
    }

    Transform GetClosestEnemy()
    {
        // Scan for enemies within the MAXIMUM range (Magic range)
        Collider[] enemies = Physics.OverlapSphere(transform.position, magicRange, enemyLayer);

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

    // Visualizes the two zones in the Editor
    void OnDrawGizmosSelected()
    {
        // Red Circle = Melee Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // Blue Circle = Magic Range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magicRange);
    }
}