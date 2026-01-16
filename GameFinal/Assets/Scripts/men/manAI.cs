using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_HesitationSafe : MonoBehaviour
{
    public float detectRange = 20f;
    public float attackRange = 2f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private EnermyAttack attack;

    private bool isDead = false;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        attack = GetComponent<EnermyAttack>();

        animator.applyRootMotion = false;
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        // 🔴 关键防崩溃判断（适配 HesitationAdapter）
        if (agent == null || !agent.enabled)
        {
            animator.SetBool("run", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // ================= 追击 =================
        if (distance <= detectRange && distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            animator.SetBool("run", true);
        }
        // ================= 攻击 =================
        else if (distance <= attackRange)
        {
            agent.isStopped = true;

            animator.SetBool("run", false);
            animator.SetTrigger("attack");

            if (attack != null)
                attack.TryAttack();
        }
        // ================= 待机 =================
        else
        {
            agent.isStopped = true;
            animator.SetBool("run", false);
        }
    }

    public void Die2()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (attack != null)
            attack.enabled = false;

        animator.SetBool("run", false);
        

        Destroy(gameObject, 3f);
    }
}
