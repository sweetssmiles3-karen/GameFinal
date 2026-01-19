using UnityEngine;

public class EnemyAttackHit : MonoBehaviour
{
    [Header("攻击设置")]
    public int damage = 2;               // 攻击力
    public float attackRange = 2f;       // 攻击有效范围

    [Header("玩家引用")]
    public Transform player;             // 玩家 Transform
    public Health playerHealth;    // 玩家血量脚本

    //  动画事件调用，触发一次伤害
    public void DealAttackDamage()
    {
        //检查玩家是否存在
        if (player == null || playerHealth == null)
        {
            Debug.LogWarning("EnemyAttackHit: 玩家未设置！");
            return;
        }

        //计算敌人和玩家的距离
        float distance = Vector3.Distance(transform.position, player.position);

        // 如果玩家在攻击范围内才造成伤害
        if (distance <= attackRange)
        {
            playerHealth.TakeDamage(damage);  // 造成伤害
            Debug.Log("Enemy攻击玩家！伤害: " + damage);
        }
        else
        {
            // 玩家不在范围内，不受伤
            Debug.Log("攻击未命中，玩家在范围外，距离: " + distance);
        }
    }
}
