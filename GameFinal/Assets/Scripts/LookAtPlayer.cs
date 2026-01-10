using UnityEngine;

public class EnemyLookAtPlayer : MonoBehaviour
{
    [Header("Target")]
    public Transform lookTarget;   // 👈 拖空物体（玩家身上的瞄准点）

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    void Update()
    {
        if (lookTarget == null) return;

        Vector3 direction = lookTarget.position - transform.position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
