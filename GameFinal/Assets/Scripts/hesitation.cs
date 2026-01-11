using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HesitationAdapter : MonoBehaviour
{
    public float hesitationTime = 2f; // 停顿时间
    public float detectRange = 20f;   // 玩家检测范围
    public GameObject player;         // 玩家引用

    private NavMeshAgent agent;
    private bool isHesitating = false;
    private bool hasHesitated = false;

    private HeadUIFlash headUIFlash; // UI脚本引用

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // 🔹 开局禁用NavMeshAgent
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p;
        }

        headUIFlash = GetComponentInChildren<HeadUIFlash>();
    }

    void Update()
    {
        if (hasHesitated || isHesitating || player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectRange)
        {
            StartCoroutine(HesitateThenEnableAgent());
        }
    }

    private IEnumerator HesitateThenEnableAgent()
    {
        isHesitating = true;

        // 🔹 调用UI闪烁
        if (headUIFlash != null)
            headUIFlash.FlashUI();

        // 🔹 等待停顿时间
        yield return new WaitForSeconds(hesitationTime);

        // 🔹 停顿结束，启用 NavMeshAgent，让 EnemyAI 自动开始追击
        if (agent != null)
            agent.enabled = true;

        hasHesitated = true;
        isHesitating = false;
    }
}
