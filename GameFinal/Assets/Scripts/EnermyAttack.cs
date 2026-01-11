using UnityEngine;

public class EnermyAttack : MonoBehaviour
{
    public int attackDamage = 2;
    public float attackCooldown = 1f;

    private float attackTimer = 0f;

    private Health playerHealth;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("❌ 找不到 PlayerHealthController");
        }
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
    }

    // 由 EnemyAI 在攻击状态时调用
    public void TryAttack()
    {
        if (playerHealth == null) return;
        if (attackTimer > 0f) return;

        attackTimer = attackCooldown;

        playerHealth.TakeDamage(attackDamage);

        Debug.Log("Enemy 攻击玩家，伤害：" + attackDamage);
    }
}
