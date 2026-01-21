using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectRange = 20f;
    public float attackRange = 2f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private EnermyAttack attack;

    private bool isDead = false;

    public AudioSource attacksound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attack = GetComponent<EnermyAttack>();


        agent.updatePosition = true;  // NavMeshAgent 控制位置
        agent.updateRotation = true;  // NavMeshAgent 控制旋转

        // Animator 不控制位置
        animator.applyRootMotion = false;

        agent.isStopped = true;
    }

    void Update()
    {
        if (isDead) return;



        float distance = Vector3.Distance(transform.position, player.position);

        // 追击状态
        if (distance <= detectRange && distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            animator.SetBool("run", true);
        }
        // 攻击状态
        else if (distance <= attackRange)
        {
            agent.isStopped = true;

            animator.SetBool("run", false);
            animator.SetTrigger("attack");

     
            if (attack != null)
                attack.TryAttack();
            attacksound.Play();
        }
        // 玩家离开
        else
        {
            agent.isStopped = true;
            animator.SetBool("run", false);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // 停止移动
        agent.isStopped = true;
        agent.enabled = false;

        // 停止攻击
        if (attack != null)
            attack.enabled = false;

        // 播放死亡动画
        animator.SetTrigger("die");

        // 可选：3 秒后删除（测试用）
        Destroy(gameObject, 3f);
    }


}
