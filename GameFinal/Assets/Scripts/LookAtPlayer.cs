using UnityEngine;

public class EnemyLookAtPlayer : MonoBehaviour
{
    public Transform player;      // 玩家 Transform
    public float rotationSpeed = 5f;  // 旋转速度，可在 Inspector 调整

    void Update()
    {
        if (player == null) return;

        // 计算面向玩家的方向
        Vector3 direction = (player.position - transform.position).normalized;

        // 只在水平面旋转，不上下仰角
        direction.y = 0;

        if (direction.magnitude > 0)
        {
            // 平滑旋转
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
