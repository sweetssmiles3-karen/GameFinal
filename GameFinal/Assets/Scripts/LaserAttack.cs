using UnityEngine;
using System.Collections;

public class LaserDamage : MonoBehaviour
{
    [Header("伤害设置")]
    public int damagePerTick = 2;       // 每次扣血
    public float tickInterval = 1f;     // 每秒扣一次血

    [Header("玩家Tag")]
    public string playerTag = "Player"; // 玩家 Tag

    // 用于防止重复触发多个协程
    private Coroutine damageCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerHealthController playerHealth = other.GetComponent<PlayerHealthController>();
            if (playerHealth != null && damageCoroutine == null)
            {
                // 开始每秒扣血
                damageCoroutine = StartCoroutine(DamageOverTime(playerHealth));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // 离开激光区域，停止扣血
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageOverTime(PlayerHealthController playerHealth)
    {
        while (true)
        {
            playerHealth.TakeDamage(damagePerTick);
            Debug.Log("Laser Attack! 扣血量: " + damagePerTick);    // 🔹 Debug 输出
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
