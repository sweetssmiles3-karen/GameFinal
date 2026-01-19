using UnityEngine;

public class StoneAttackEffect : MonoBehaviour
{
    [Header("攻击特效物体")]
    public GameObject attackEffect;

    [Header("特效持续时间（秒）")]
    public float effectDuration = 1f; // 播放多久自动隐藏

    private Coroutine currentEffectCoroutine;

    void Start()
    {
        if (attackEffect != null)
            attackEffect.SetActive(false); // 开局隐藏
    }

    // 动画事件调用
    public void PlayAttackEffect()
    {
        if (attackEffect == null) return;

        //  如果上一次还在播放，先停止
        if (currentEffectCoroutine != null)
            StopCoroutine(currentEffectCoroutine);

        //  重置状态，保证每次都能播放
        attackEffect.SetActive(false);
        attackEffect.SetActive(true);

        //  自动隐藏特效
        currentEffectCoroutine = StartCoroutine(HideEffectAfterDelay(effectDuration));
    }

    private System.Collections.IEnumerator HideEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (attackEffect != null)
            attackEffect.SetActive(false);
    }
}
