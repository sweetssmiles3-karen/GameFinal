using UnityEngine;
using UnityEngine.Events;
using System.Collections; 

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Player Stats")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Knockback Settings")]
    public float knockbackDistance = 2f; 
    public float knockbackDuration = 0.2f; 

    [Header("Combat Flags")]
    public bool isInvulnerable = false;

    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnPlayerDeath;

    private Animator anim;
    private PlayerMovement movement;
    private PlayerCombat combat;
    private CharacterController characterController; 
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        characterController = GetComponent<CharacterController>(); 

        OnHealthChanged?.Invoke(1f);
    }

    public void TakeDamage(float amount)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            
            if (anim != null) anim.SetTrigger("GetHit");

            
            StopCoroutine("PerformKnockback"); 
            StartCoroutine(PerformKnockback());
        }
    }

    IEnumerator PerformKnockback()
    {
        
        if (movement != null) movement.enabled = false;
        if (combat != null) combat.enabled = false;

        float timer = 0f;

        
        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;

            
            float speed = knockbackDistance / knockbackDuration;
            Vector3 pushDir = -transform.forward * speed * Time.deltaTime;

            
            if (characterController != null)
            {
               
                characterController.Move(pushDir);
            }
            else
            {
                
                transform.position += pushDir;
            }

            yield return null; 
        }

        
        if (!isDead)
        {
            if (movement != null) movement.enabled = true;
            if (combat != null) combat.enabled = true;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
        {
            anim.ResetTrigger("GetHit");
            anim.SetTrigger("Die");
            anim.SetFloat("Speed", 0f);
        }

        OnPlayerDeath?.Invoke();

       
        if (movement != null) movement.enabled = false;
        if (combat != null) combat.enabled = false;
        if (characterController != null) characterController.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("Player Died.");
    }
}