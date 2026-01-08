using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectRange = 20f;
    public float attackRange = 2f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updatePosition = true;  // NavMeshAgent ¿ØÖÆÎ»ÖÃ
        agent.updateRotation = true;  // NavMeshAgent ¿ØÖÆÐý×ª

        // Animator ²»¿ØÖÆÎ»ÖÃ
        animator.applyRootMotion = false;

        agent.isStopped = true;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // ×·»÷×´Ì¬
        if (distance <= detectRange && distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            animator.SetBool("run", true);
        }
        // ¹¥»÷×´Ì¬
        else if (distance <= attackRange)
        {
            agent.isStopped = true;

            animator.SetBool("run", false);
            animator.SetTrigger("attack");
        }
        // Íæ¼ÒÀë¿ª
        else
        {
            agent.isStopped = true;
            animator.SetBool("run", false);
        }
    }
}
