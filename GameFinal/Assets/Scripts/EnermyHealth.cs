using UnityEngine;

public class EnermyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 20;   // Inspector 可改

    private int currentHP;

    private EnemyAI enemyAI;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;


    void Start()
    {
        currentHP = maxHP;

        enemyAI = GetComponent<EnemyAI>();
        if (enemyAI == null)
        {
            Debug.LogError("❌ EnermyHealth 找不到 EnemyAI");
        }
    }

    // 被外部调用（比如玩家攻击）
    public void TakeDamage(int damage)
    {
        if (currentHP <= 0) return;

        currentHP -= damage;
        Debug.Log("Enemy 受伤：" + damage + " | 当前血量：" + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 Enemy 死亡");

        if (enemyAI != null)
        {
            enemyAI.Die();   // 🔥 调用你刚刚写的死亡逻辑
        }
    }
}
