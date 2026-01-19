using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviour
{
    [Header("Audio SFX - Cast")]
    public AudioClip[] comboSounds;
    public AudioClip meleeAttackSound;
    public AudioClip skillGSound;
    public AudioClip reloadSound;

    [Header("Audio SFX - Impacts")]
    public AudioClip rangedImpactSound;
    public AudioClip lightningHitSound;
    public AudioClip grenadeExplosionSound;

    [Header("Mana Settings")]
    public float maxMana = 40f;
    public float currentMana;
    public float leftClickManaCost = 1f;
    public float rightClickManaCost = 5f;

    [Header("Mana Regeneration")]
    public float manaRegenAmount = 1f;
    public float manaRegenInterval = 1.0f;
    public UnityEvent<float> OnManaChanged;

    [Header("Combo Speed (Cooldowns)")]
    public float[] comboIntervals = { 0.5f, 0.7f, 1.0f };
    public float comboResetTime = 2.0f;

    [Header("Left Click (Ranged Combo)")]
    public float rangedDamage = 20f;
    public float rangedRange = 15f;
    public GameObject rangedHitEffect;
    public float[] rangedImpactDelays = { 0.3f, 0.3f, 0.5f };

    [Header("Right Click (Melee Lightning)")]
    public float meleeDamage = 50f;
    public float meleeRange = 5f;
    public int maxMeleeTargets = 3;
    public float meleeLockTime = 2.0f;
    public GameObject meleeHitEffect;
    public float meleeImpactDelay = 0.5f;
    public float lightningYOffset = 0f;

    [Header("Skill G (Laser Bomb)")]
    public float grenadeDamage = 80f;
    public float grenadeRadius = 4f;
    public float grenadeCastDelay = 0.5f;
    public GameObject grenadeEffect;
    public float grenadeDistance = 3.0f;
    public float grenadeManaCost = 20f;
    public float grenadeCooldown = 5.0f;

    [Header("General Settings")]
    public LayerMask enemyLayer;

    private Animator anim;
    private PlayerMovement movement;
    private PlayerHealth myHealth;
    private AudioSource audioSource;

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private float currentCooldown = 0f;
    private bool isAttacking = false;
    private bool isReloading = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        myHealth = GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();

        currentMana = maxMana;
        OnManaChanged?.Invoke(currentMana / maxMana);
    }

    void Update()
    {
        // 1. MANA REGEN CANCELLATION
        if (isReloading)
        {
            // If player moves, clicks, or casts G, stop reloading
            bool isMoving = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
            bool isClicking = (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.G));

            if (isMoving || isClicking)
            {
                StopReloading();
            }
        }

        if (Time.time - lastAttackTime > comboResetTime && comboStep != 0)
        {
            comboStep = 0;
            anim.SetInteger("ComboInt", 0);
        }

        if (isAttacking) return;
        if (Time.time < lastAttackTime + currentCooldown) return;

        // LEFT CLICK
        if (Input.GetMouseButtonDown(0))
        {
            if (currentMana >= leftClickManaCost)
            {
                currentMana -= leftClickManaCost;
                OnManaChanged?.Invoke(currentMana / maxMana);
                StopReloading();
                StartCoroutine(PerformRangedCombo());
            }
            else
            {
                Debug.Log("Not enough Mana!");
            }
        }

        // RIGHT CLICK
        if (Input.GetMouseButtonDown(1))
        {
            if (currentMana >= rightClickManaCost)
            {
                currentMana -= rightClickManaCost;
                OnManaChanged?.Invoke(currentMana / maxMana);
                StopReloading();
                StartCoroutine(PerformMeleeAttack());
            }
            else
            {
                Debug.Log("Not enough Mana!");
            }
        }

        // RELOAD (Key R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Only start if not reloading AND mana is not full
            if (!isReloading && currentMana < maxMana)
            {
                StartCoroutine(ReloadManaRoutine());
            }
        }

        // G SKILL
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentMana >= grenadeManaCost)
            {
                currentMana -= grenadeManaCost;
                OnManaChanged?.Invoke(currentMana / maxMana);
                StopReloading();
                StartCoroutine(PerformGrenadeSkill());
            }
            else
            {
                Debug.Log("Not enough Mana for G-Bomb!");
            }
        }
    }

    // --- MANA REGEN LOGIC (UPDATED FOR LOOP) ---
    IEnumerator ReloadManaRoutine()
    {
        isReloading = true;
        lastAttackTime = Time.time;
        currentCooldown = 0.5f;

        // 1. START ANIMATION LOOP
        // Note: You must create a Bool parameter named "IsReloading" in Animator!
        anim.SetBool("IsReloading", true);

        // 2. START SOUND LOOP
        if (audioSource != null && reloadSound != null)
        {
            audioSource.clip = reloadSound;
            audioSource.loop = true; // Make it repeat continuously
            audioSource.Play();
        }

        // 3. REGEN LOOP
        while (isReloading && currentMana < maxMana)
        {
            yield return new WaitForSeconds(manaRegenInterval);

            if (isReloading)
            {
                currentMana += manaRegenAmount;
                // If full, cap it and stop
                if (currentMana >= maxMana)
                {
                    currentMana = maxMana;
                    OnManaChanged?.Invoke(currentMana / maxMana);
                    StopReloading(); // <--- Auto-stop when full
                    yield break;     // Exit the loop
                }

                OnManaChanged?.Invoke(currentMana / maxMana);
            }
        }
    }

    void StopReloading()
    {
        if (isReloading)
        {
            isReloading = false;
            StopCoroutine("ReloadManaRoutine");

            // 1. STOP ANIMATION
            anim.SetBool("IsReloading", false);

            // 2. STOP SOUND
            if (audioSource != null)
            {
                audioSource.loop = false;
                audioSource.Stop();
            }
        }
    }

    // --- COMBAT COROUTINES (SAME AS BEFORE) ---

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

        if (audioSource != null && comboSounds.Length >= comboStep)
        {
            int soundIndex = comboStep - 1;
            if (comboSounds[soundIndex] != null) audioSource.PlayOneShot(comboSounds[soundIndex]);
        }

        float currentDelay = 0.3f;
        if (rangedImpactDelays.Length >= 3) currentDelay = rangedImpactDelays[comboStep - 1];
        yield return new WaitForSeconds(currentDelay);

        Transform target = GetClosestEnemy(rangedRange);

        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);

            if (rangedHitEffect != null)
                Instantiate(rangedHitEffect, target.position + Vector3.up, Quaternion.identity);

            if (audioSource != null && rangedImpactSound != null)
            {
                audioSource.PlayOneShot(rangedImpactSound);
            }

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

        if (audioSource != null && meleeAttackSound != null) audioSource.PlayOneShot(meleeAttackSound);

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

                    if (audioSource != null && lightningHitSound != null)
                    {
                        audioSource.PlayOneShot(lightningHitSound);
                    }

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

    IEnumerator PerformGrenadeSkill()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        currentCooldown = grenadeCooldown;

        anim.SetTrigger("Grenade");

        if (audioSource != null && skillGSound != null) audioSource.PlayOneShot(skillGSound);

        if (movement != null) movement.canMove = false;

        yield return new WaitForSeconds(grenadeCastDelay);

        Vector3 spawnPos = transform.position + (transform.forward * grenadeDistance);
        spawnPos.y += 1.0f;

        if (grenadeEffect != null) Instantiate(grenadeEffect, spawnPos, Quaternion.identity);

        if (audioSource != null && grenadeExplosionSound != null)
        {
            audioSource.PlayOneShot(grenadeExplosionSound);
        }

        Collider[] hitEnemies = Physics.OverlapSphere(spawnPos, grenadeRadius, enemyLayer);
        foreach (Collider collider in hitEnemies)
        {
            IDamageable target = collider.GetComponent<IDamageable>();
            if (target != null) target.TakeDamage(grenadeDamage);
        }

        yield return new WaitForSeconds(0.5f);

        if (movement != null) movement.canMove = true;
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