using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BossHealth_UI))] // 之前写的血量脚本
[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    [Header("追踪设置")]
    public Transform player;
    public float detectRange = 15f; // 检测玩家范围

    [Header("攻击冷却")]
    public float attackFarCooldown = 2f; // 远程攻击间隔
    public float attackNearCooldown = 1.5f; // 近战攻击间隔

    [Header("近战伤害设置")]
    public int meleeDamage = 10;              // 每次近战伤害
    public float meleeCooldown = 2f;          // 攻击冷却
    public LaserDamage laserAttack;           // Phase1 的 LaserDamage 脚本引用
    public float meleeRange = 2f;              // 近战攻击范围
    private bool canMeleeAttack = true;       // 攻击冷却标记


    private Animator animator;
    private BossHealth_UI bossHealth;
    private NavMeshAgent agent;

    private bool isPhase2 = false;
    private bool isDead = false;

    private float nextAttackTime = 0f; // 控制攻击间隔

    void Start()
    {
        animator = GetComponent<Animator>();       //  Animator引用
        bossHealth = GetComponent<BossHealth_UI>(); //  血量引用
        agent = GetComponent<NavMeshAgent>();      //  NavMeshAgent引用

        agent.enabled = false; //  初始不开启NavMesh（Phase1远程攻击原地）
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        //Phase1逻辑
        if (!isPhase2)
        {
            if (distance <= detectRange && Time.time >= nextAttackTime)
            {
                Vector3 lookDir = player.position - transform.position;
                

                if (lookDir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(lookDir);

                animator.SetTrigger("attackFar");
                nextAttackTime = Time.time + attackFarCooldown;
            }

            if (!isPhase2 && bossHealth != null && bossHealth.currentHP <= 0 && !isDead)
            {
                StartCoroutine(EnterPhase2());
            }
        }

        //Phase2逻辑
        if (isPhase2 && !isDead)
        {
            // PowerUp 完成后（Phase2开始）
            if (laserAttack != null)
                laserAttack.enabled = false; // 禁用原来的激光伤害


            if (agent.enabled)
            {
                agent.SetDestination(player.position);

                if (animator != null)
                    animator.SetBool("run", agent.velocity.sqrMagnitude > 0.1f);
            }

            if (distance <= detectRange && Time.time >= nextAttackTime)
            {
                animator.SetTrigger("attackNear");
                nextAttackTime = Time.time + attackNearCooldown;
                if(distance<=meleeRange)
                // 🔹 延迟伤害：可以用协程或 Animator Event
                StartCoroutine(DelayedMeleeHit(0.5f)); // 假设动画0.5秒后打击命中
            }

            if (bossHealth != null && bossHealth.currentHP <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }


    /// <summary>
    /// Phase1 -> Phase2切换
    /// </summary>
    private IEnumerator EnterPhase2()
    {
        isPhase2 = true;

        //  禁用 NavMeshAgent，防止移动
        if (agent != null)
            agent.enabled = false;

        //  设置 PowerUp 动画播放速度为 0.5 倍
        if (animator != null)
        {
            animator.speed = 0.5f;           //  全局动画速度减半
            animator.SetTrigger("PowerUp");
        }

        //  血量回满
        if (bossHealth != null)
        {
            bossHealth.isPhase2 = true;
            bossHealth.currentHP = bossHealth.maxHPPhase2;
            bossHealth.UpdateHealthUI();
        }

        //  等待 PowerUp 动画播放完（假设动画长度 3 秒，0.5倍速需要 6 秒）
        float powerUpLength = 3f;            // 原始动画长度
        yield return new WaitForSeconds(powerUpLength / 0.5f); // 0.5倍速播放时间 = 原始 / 0.5 = 6秒

        //  恢复动画速度为正常
        if (animator != null)
            animator.speed = 1f;

        //  开启 NavMeshAgent，开始追玩家
        if (agent != null)
            agent.enabled = true;
    }


    /// <summary>
    /// Boss死亡处理
    /// </summary>
    private IEnumerator Die()
    {
        isDead = true;

        //  禁用NavMesh
        if (agent != null)
            agent.enabled = false;

        //  播放死亡动画
        if (animator != null)
            animator.SetTrigger("Dead");

        //  等待3秒后删除
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void PerformMeleeAttack()
    {
        if (!canMeleeAttack) return;

        // 找到玩家血量脚本
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
   
        
            playerHealth.TakeDamage(meleeDamage);
            Debug.Log($"💥 Boss近战攻击玩家 -{meleeDamage}HP");
        

        // 设置攻击冷却
        canMeleeAttack = false;
        StartCoroutine(MeleeCooldown());
    }

    private IEnumerator MeleeCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);
        canMeleeAttack = true;
    }

    private IEnumerator DelayedMeleeHit(float delay)
    {
        yield return new WaitForSeconds(delay);
        PerformMeleeAttack();
    }


}
