using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public int maxHP = 20;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
        Debug.Log("Player HP: " + currentHP);
    }

    // 被敌人调用的扣血方法
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("Player 被攻击，扣血：" + damage +
                  " | 当前血量：" + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 Player 死亡");
    }
}
