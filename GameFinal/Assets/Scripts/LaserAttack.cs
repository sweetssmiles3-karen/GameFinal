using UnityEngine;
using System.Collections;

public class LaserDamage : MonoBehaviour
{
    [Header("伤害设置")]
    public float damagePerTick = 2f;     // 每次扣血
    public float tickInterval = 1f;      // 每秒扣一次血

    [Header("玩家Tag")]
    public string playerTag = "Player";

    private Coroutine damageCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageOverTime(playerHealth));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageOverTime(PlayerHealth playerHealth)
    {
        while (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerTick);
            Debug.Log("🔥 Laser Attack! 扣血量: " + damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
