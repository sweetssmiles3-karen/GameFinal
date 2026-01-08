using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class HesitationSimple : MonoBehaviour
{
    public float hesitationTime = 2f; // 停多久
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (agent != null)
        {
            agent.enabled = false; // 开局禁用 NavMeshAgent
            StartCoroutine(EnableAgentAfterDelay());
        }
    }

    private IEnumerator EnableAgentAfterDelay()
    {
        yield return new WaitForSeconds(hesitationTime);
        if (agent != null)
            agent.enabled = true; // 两秒后恢复
    }


}
