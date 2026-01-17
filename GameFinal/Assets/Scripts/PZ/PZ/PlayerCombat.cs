using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combo Speed Settings")]
    public float[] comboIntervals = { 0.5f, 0.7f, 1.0f };
    public float comboResetTime = 2.0f;

    [Header("Left Click (Ranged Single-Hit)")]
    public float rangedDamage = 20f;
    public float rangedRange = 15f;
    public GameObject rangedHitEffect;
    public float[] rangedImpactDelays = { 0.3f, 0.3f, 0.5f };

    [Header("Right Click (Melee Lightning Strike)")]
    public float meleeDamage = 50f;
    public float meleeRange = 5f;
    public int maxMeleeTargets = 3;
    public float meleeLockTime = 2.0f;
    public GameObject meleeHitEffect;
    public float meleeImpactDelay = 0.5f;
    public float lightningYOffset = 0f;

    [Header("Skill G (Laser Bomb)")]
    public float grenadeDamage = 80f;        // High damage
    public float grenadeRadius = 4f;         // Size of the explosion area
    public float grenadeCastDelay = 0.5f;    // Time before effect appears
    public GameObject grenadeEffect;         // Drag your BIG LASER SPHERE here!
    public float grenadeDistance = 3.0f;     // How far in front it spawns

    [Header("General Settings")]
    public LayerMask enemyLayer;

    // Internal Variables
    private Animator anim;
    private PlayerMovement movement;
    private Health myHealth;

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private float currentCooldown = 0f;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        myHealth = GetComponent<Health>();
    }

    void Update()
    {
        if (Time.time - lastAttackTime > comboResetTime && comboStep != 0)
        {
            comboStep = 0;
            anim.SetInteger("ComboInt", 0);
        }

        if (isAttacking) return;
        if (Time.time < lastAttackTime + currentCooldown) return;

        // 1. LEFT CLICK
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PerformRangedCombo());
        }

        // 2. RIGHT CLICK
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(PerformMeleeAttack());
        }

        // 3. SKILL R
        if (Input.GetKeyDown(KeyCode.R))
        {
            lastAttackTime = Time.time;
            currentCooldown = 1.0f;
            anim.SetTrigger("Reload");
        }

        // 4. SKILL G (BIG LASER BOMB)
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(PerformGrenadeSkill());
        }
    }

    IEnumerator PerformGrenadeSkill()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        currentCooldown = 2.0f; // Long cooldown for powerful skill

        // Trigger Animation
        anim.SetTrigger("Grenade");

        // Lock Movement? (Optional, remove if you want to walk while casting)
        if (movement != null) movement.canMove = false;

        // Wait for the "Throw" or "Cast" moment in animation
        yield return new WaitForSeconds(grenadeCastDelay);

        // --- SPAWN LASER BOMB LOGIC ---
        // Calculate position in front of player
        Vector3 spawnPos = transform.position + (transform.forward * grenadeDistance);
        spawnPos.y += 1.0f; // Lift it off the ground slightly

        // 1. Spawn the Visual Effect
        if (grenadeEffect != null)
        {
            Instantiate(grenadeEffect, spawnPos, Quaternion.identity);
        }

        // 2. Deal Area Damage
        Collider[] hitEnemies = Physics.OverlapSphere(spawnPos, grenadeRadius, enemyLayer);
        foreach (Collider collider in hitEnemies)
        {
            IDamageable target = collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(grenadeDamage);
            }
        }

        // Wait a bit before allowing movement again
        yield return new WaitForSeconds(0.5f);

        if (movement != null) movement.canMove = true;
        isAttacking = false;
    }

    // ... [PerformRangedCombo and PerformMeleeAttack stay the same] ...

    // (Paste the previous PerformRangedCombo, PerformMeleeAttack, and GetClosestEnemy functions here)
    // I am omitting them to save space, but DO NOT DELETE THEM from your file!

    IEnumerator PerformRangedCombo()
    {
        anim.ResetTrigger("Attack");
        lastAttackTime = Time.time;
        comboStep++;
        if (comboStep > 3) comboStep = 1;
        if (comboIntervals.Length >= 3) currentCooldown = comboIntervals[comboStep - 1];
        else currentCooldown = 0.5f;
        anim.SetInteger("ComboInt", comboStep);
        anim.SetTrigger("Attack");

        float currentDelay = 0.3f;
        if (rangedImpactDelays.Length >= 3) currentDelay = rangedImpactDelays[comboStep - 1];
        yield return new WaitForSeconds(currentDelay);

        Transform target = GetClosestEnemy(rangedRange);
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
            if (rangedHitEffect != null) Instantiate(rangedHitEffect, target.position + Vector3.up, Quaternion.identity);
            float dmg = rangedDamage;
            if (comboStep == 3) dmg *= 2;
            IDamageable enemyHealth = target.GetComponent<IDamageable>();
            if (enemyHealth != null) enemyHealth.TakeDamage(dmg);
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        currentCooldown = 1.0f;
        comboStep = 0;
        anim.SetInteger("ComboInt", 0);
        if (movement != null) movement.canMove = false;
        if (myHealth != null) myHealth.isInvulnerable = true;
        anim.SetTrigger("CriticalAttack");
        yield return new WaitForSeconds(meleeImpactDelay);
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, meleeRange, enemyLayer);
        List<GameObject> alreadyHitList = new List<GameObject>();
        int hitCount = 0;
        foreach (Collider collider in hitEnemies)
        {
            if (hitCount >= maxMeleeTargets) break;
            GameObject enemyObject = collider.gameObject;
            if (!alreadyHitList.Contains(enemyObject))
            {
                IDamageable target = collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(meleeDamage);
                    if (meleeHitEffect != null)
                    {
                        Vector3 spawnPos = enemyObject.transform.position + (Vector3.up * lightningYOffset);
                        Instantiate(meleeHitEffect, spawnPos, meleeHitEffect.transform.rotation);
                    }
                    alreadyHitList.Add(enemyObject);
                    hitCount++;
                }
            }
        }
        float remainingWait = meleeLockTime - meleeImpactDelay;
        if (remainingWait > 0) yield return new WaitForSeconds(remainingWait);
        if (movement != null) movement.canMove = true;
        if (myHealth != null) myHealth.isInvulnerable = false;
        isAttacking = false;
    }

    Transform GetClosestEnemy(float range)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, range, enemyLayer);
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
}