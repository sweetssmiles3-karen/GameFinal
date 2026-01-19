using UnityEngine;

public class HealthTester : MonoBehaviour
{
    private PlayerHealth playerHealth;

    void Start()
    {
        
        playerHealth = GetComponent<PlayerHealth>();

        
        if (playerHealth == null)
        {
            Debug.LogError("TESTER ERROR: You forgot to attach 'PlayerHealth' to this object!");
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Testing: Player Flinch (GetHit)");
            if (playerHealth != null)
            {
                
                playerHealth.TakeDamage(10f);
            }
        }

        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Testing: Player Death (Die)");
            if (playerHealth != null)
            {
                
                playerHealth.TakeDamage(playerHealth.maxHealth + 1000f);
            }
        }
    }
}