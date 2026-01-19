using UnityEngine;
using System.Collections;

public class EnemyAttackEffectController : MonoBehaviour
{
    [System.Serializable]
    public class EffectEntry
    {
        public GameObject effect;   // 特效物体
        public float delay = 0f;    // 播放延迟，单位秒
    }

    [Header("Attack Effects")]
    public EffectEntry[] attackEffects; // 可以拖多个，每个设置不同延迟

    void Start()
    {
        // 开局关闭所有特效
        if (attackEffects != null)
        {
            foreach (var entry in attackEffects)
            {
                if (entry.effect != null)
                    entry.effect.SetActive(false);
            }
        }
    }

    //  动画事件调用：开始播放所有特效
    public void PlayAttackEffect()
    {
        if (attackEffects != null)
        {
            foreach (var entry in attackEffects)
            {
                if (entry.effect != null)
                    StartCoroutine(PlayEffectWithDelay(entry.effect, entry.delay));
            }
        }
    }

    //  动画事件调用：停止所有特效
    public void StopAttackEffect()
    {
        if (attackEffects != null)
        {
            foreach (var entry in attackEffects)
            {
                if (entry.effect != null)
                {
                    entry.effect.SetActive(false);
                }
            }
        }
    }

    private IEnumerator PlayEffectWithDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(true);
    }
}
