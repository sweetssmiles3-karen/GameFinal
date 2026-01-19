using UnityEngine;

public class StoneHealth : MonoBehaviour, IDamageable
{
    [Header("Stone Health Settings")]
    public int maxHP = 50;

    private int currentHP;
    private StoneAI stoneAI;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    void Start()
    {
        currentHP = maxHP;

        stoneAI = GetComponent<StoneAI>();
        if (stoneAI == null)
        {
            Debug.LogError("❌ StoneHealth 找不到 StoneAI");
        }
    }

    // 被玩家或子弹调用
    public void TakeDamage(float damagef)
    {
        if (currentHP <= 0) return;
        int damage = Mathf.RoundToInt(damagef);
        currentHP -= damage;
        Debug.Log($"🪨 Stone Enemy 受伤：{damage} | HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("💀 Stone Enemy 死亡");

        if (stoneAI != null)
        {
            stoneAI.Die();
        }
    }
}
