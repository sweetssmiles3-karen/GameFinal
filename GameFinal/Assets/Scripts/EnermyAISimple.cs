using UnityEngine;

public class EnemyAI_SimpleAttack : MonoBehaviour
{
    public float attackRange = 3f;

    private Transform player;
    private Animator animator;

    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
    }

    // 👉 给 Animation Event 用（攻击动画最后一帧调用）
    public void EndAttack()
    {
        isAttacking = false;
    }
}
