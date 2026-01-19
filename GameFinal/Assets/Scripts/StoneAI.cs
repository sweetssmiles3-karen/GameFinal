using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class StoneAI : MonoBehaviour
{
    public float detectRange = 20f;
    public float attackRange = 2f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private EnermyAttack attack;

    private bool isDead = false;
    private bool isAttacking = false; // 标记攻击中

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attack = GetComponent<EnermyAttack>();

        agent.updatePosition = true;
        agent.updateRotation = true;
        animator.applyRootMotion = false;

        agent.isStopped = true;
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 如果正在攻击，NavMeshAgent 已经暂停，这里不改变位置
        if (isAttacking)
        {
            // 攻击中什么都不做，动画会继续
            return;
        }

        // 追击状态
        if (distance <= detectRange && distance > attackRange)
        {
            agent.isStopped = false;
            if (agent.isOnNavMesh) //  安全调用
                agent.SetDestination(player.position);

            animator.SetBool("run", true);
        }
        // 攻击状态
        else if (distance <= attackRange)
        {
            agent.isStopped = true;       //  攻击时停止移动
            animator.SetBool("run", false);
            animator.SetTrigger("attack");

            if (attack != null)
                attack.TryAttack();

            // 🔹 标记攻击中，避免重复触发
            isAttacking = true;

            // 🔹 攻击结束后恢复 NavMeshAgent
            // 可以改 delay = 动画长度
            StartCoroutine(EndAttackAfterDelay(7.533f));
        }
        // 玩家离开
        else
        {
            agent.isStopped = true;
            animator.SetBool("run", false);
        }
    }

    private IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isDead && agent != null)
            agent.isStopped = false;

        isAttacking = false; // 攻击结束
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // 停止移动
        if (agent != null) agent.isStopped = true;
        if (agent != null) agent.enabled = false;

        // 停止攻击
        if (attack != null)
            attack.enabled = false;

        // 播放死亡动画
        animator.SetTrigger("die");

        // 可选：3 秒后删除（测试用）
        Destroy(gameObject, 3f);
    }
}
