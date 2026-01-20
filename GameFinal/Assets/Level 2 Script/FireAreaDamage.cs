using System.Collections;
using UnityEngine;

public class FireAreaDamage : MonoBehaviour
{
    public int damagePerTick = 5; // 持续伤害小
    public float interval = 1f;    // 每秒扣血一次

    private Coroutine damageCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        damageCoroutine = StartCoroutine(DamageLoop(other));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator DamageLoop(Collider player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health == null) yield break;

        while (true)
        {
            health.TakeDamage(damagePerTick);
            Debug.Log($"[FireArea] {player.name} takes {damagePerTick} damage!");
            yield return new WaitForSeconds(interval);
        }
    }
}
