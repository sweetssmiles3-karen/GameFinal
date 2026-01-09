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

    private Animator animator;
    private BossHealth_UI bossHealth;
    private NavMeshAgent agent;

    private bool isPhase2 = false;
    private bool isDead = false;

    private float nextAttackTime = 0f; // 控制攻击间隔

    void Start()
    {
        animator = GetComponent<Animator>();       // 🔹 Animator引用
        bossHealth = GetComponent<BossHealth_UI>(); // 🔹 血量引用
        agent = GetComponent<NavMeshAgent>();      // 🔹 NavMeshAgent引用

        agent.enabled = false; // 🔹 初始不开启NavMesh（Phase1远程攻击原地）
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // ---------- Phase1逻辑 ----------
        if (!isPhase2)
        {
            if (distance <= detectRange && Time.time >= nextAttackTime)
            {
                // 🔹【新增】攻击前先面向玩家
                Vector3 lookDir = player.position - transform.position;
                

                if (lookDir.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                // 🔹 播放远程攻击动画
                animator.SetTrigger("attackFar");

                // 🔹 设置攻击冷却
                nextAttackTime = Time.time + attackFarCooldown;
            }

            // 血量归零时进入Phase2
            if (bossHealth != null && bossHealth.currentHP <= 0)
            {
                StartCoroutine(EnterPhase2());
            }
        }

        // ---------- Phase2逻辑 ----------
        if (isPhase2)
        {
            // 开启NavMesh追踪玩家
            if (agent.enabled)
                agent.SetDestination(player.position);

            // 检测玩家距离进行近战攻击
            if (distance <= detectRange && Time.time >= nextAttackTime)
            {
                animator.SetTrigger("attackNear");      // 🔹 播放近战攻击动画
                nextAttackTime = Time.time + attackNearCooldown;
            }

            // Phase2血量归零死亡
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

        // 🔹 播放PowerUp动画
        if (animator != null)
            animator.SetTrigger("PowerUp");

        // 🔹 血量回满
        if (bossHealth != null)
        {
            bossHealth.isPhase2 = true;
            bossHealth.currentHP = bossHealth.maxHPPhase2;
            bossHealth.UpdateHealthUI();
        }

        // 🔹 等待PowerUp动画播放完（假设3秒）
        yield return new WaitForSeconds(3f);

        // 🔹 开启NavMesh，开始追玩家
        if (agent != null)
            agent.enabled = true;
    }

    /// <summary>
    /// Boss死亡处理
    /// </summary>
    private IEnumerator Die()
    {
        isDead = true;

        // 🔹 禁用NavMesh
        if (agent != null)
            agent.enabled = false;

        // 🔹 播放死亡动画
        if (animator != null)
            animator.SetTrigger("Dead");

        // 🔹 等待3秒后删除
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
