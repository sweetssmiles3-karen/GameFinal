using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    public int damage = 15; // 爆炸一次性大伤害

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"[Explosion] {other.name} takes {damage} damage!");
        }
    }
}
