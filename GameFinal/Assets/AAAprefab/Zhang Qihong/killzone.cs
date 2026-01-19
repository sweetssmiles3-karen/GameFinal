using UnityEngine;

public class killzone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int damageAmount = 9999;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 重新加载当前场景
            other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }
    }

}

