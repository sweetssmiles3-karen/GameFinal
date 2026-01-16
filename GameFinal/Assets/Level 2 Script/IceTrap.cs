using System.Collections;
using UnityEngine;

public class IceTrap : MonoBehaviour
{
    [Header("VFX Objects")]
    public GameObject magicCircle;
    public GameObject iceAttackVFX;
    public GameObject iceAreaVFX;

    [Header("Damage / Slow Zone")]
    public GameObject iceArea;  

    [Header("Audio Sources")]
    public AudioSource warningAudio;
    public AudioSource iceAttackAudio;
    public AudioSource iceLoopAudio;

    [Header("Audio Clips")]
    public AudioClip warningSound;
    public AudioClip iceAttackSound;
    public AudioClip iceLoopSound;

    [Header("Timing")]
    public float attackDelay = 1.5f;
    public float iceAreaDuration = 4f;
    public float iceLoopFadeTime = 1.5f;
    public float damageInterval = 1f; 

    [Header("Damage / Slow Settings")]
    public int iceDamage = 5;          
    public float slowPercent = 0.5f;     // 减速比例（0-1）

    private bool triggered = false;
    private Coroutine slowCoroutine;
    private Coroutine iceLoopFadeCoroutine;
    private Coroutine damageCoroutine;

    private void Start()
    {
        if (magicCircle != null) magicCircle.SetActive(false);
        if (iceAttackVFX != null) iceAttackVFX.SetActive(false);
        if (iceAreaVFX != null) iceAreaVFX.SetActive(false);
        if (iceArea != null) iceArea.SetActive(false);
    }

    // 玩家第一次踩触发器
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 显示法阵 + 播放预警音
        if (magicCircle != null) magicCircle.SetActive(true);
        if (warningAudio != null && warningSound != null)
            warningAudio.PlayOneShot(warningSound);

        StartCoroutine(TriggerIceAttack());
    }

    // 冰攻击流程
    private IEnumerator TriggerIceAttack()
    {
        yield return new WaitForSeconds(attackDelay);

        if (magicCircle != null) magicCircle.SetActive(false);

        // 播放冰攻击特效和音效
        if (iceAttackVFX != null) iceAttackVFX.SetActive(true);
        if (iceAttackAudio != null && iceAttackSound != null)
            iceAttackAudio.PlayOneShot(iceAttackSound);

        // 激活冰区（伤害 + 减速）
        if (iceArea != null) iceArea.SetActive(true);
        if (iceAreaVFX != null) iceAreaVFX.SetActive(true);

        // 冰区持续 iceAreaDuration 秒后消失
        yield return new WaitForSeconds(iceAreaDuration);

        if (iceArea != null) iceArea.SetActive(false);
        if (iceAreaVFX != null) iceAreaVFX.SetActive(false);

        // 停止持续伤害和减速协程
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        // 淡出循环音
        if (iceLoopAudio != null && iceLoopAudio.isPlaying)
        {
            if (iceLoopFadeCoroutine != null) StopCoroutine(iceLoopFadeCoroutine);
            iceLoopFadeCoroutine = StartCoroutine(FadeOutIceLoop(iceLoopFadeTime));
        }
    }

    // 玩家在冰区持续减速 + 持续伤害
    private void OnTriggerStay(Collider other)
    {
        if (!triggered || iceArea == null || !iceArea.activeSelf) return;
        if (!other.CompareTag("Player")) return;

        // 播放冰区循环音（只要玩家在范围内）
        if (iceLoopAudio != null && iceLoopSound != null && !iceLoopAudio.isPlaying)
        {
            if (iceLoopFadeCoroutine != null)
            {
                StopCoroutine(iceLoopFadeCoroutine); // 停止淡出协程
                iceLoopAudio.volume = 1f;
            }

            iceLoopAudio.clip = iceLoopSound;
            iceLoopAudio.loop = true;
            iceLoopAudio.Play();
        }

        // 持续减速
        if (slowCoroutine == null)
            slowCoroutine = StartCoroutine(SlowEffectCoroutine(other));

        // 持续伤害
        if (damageCoroutine == null)
            damageCoroutine = StartCoroutine(ApplyIceDamage(other));
    }

    // 玩家离开冰区，停止减速、伤害并淡出音效
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        if (iceLoopAudio != null && iceLoopAudio.isPlaying)
        {
            if (iceLoopFadeCoroutine != null) StopCoroutine(iceLoopFadeCoroutine);
            iceLoopFadeCoroutine = StartCoroutine(FadeOutIceLoop(iceLoopFadeTime));
        }
    }

    // 持续减速协程
    private IEnumerator SlowEffectCoroutine(Collider player)
    {
        while (iceArea != null && iceArea.activeSelf)
        {
            yield return new WaitForSeconds(0.5f);
        }
        slowCoroutine = null;
    }

    // 持续伤害协程
    private IEnumerator ApplyIceDamage(Collider player)
    {
        Health health = player.GetComponent<Health>();
        if (health == null) yield break;

        while (iceArea != null && iceArea.activeSelf)
        {
            health.TakeDamage(iceDamage);
            yield return new WaitForSeconds(damageInterval);
        }
        damageCoroutine = null;
    }

    // 循环音淡出
    private IEnumerator FadeOutIceLoop(float duration)
    {
        float startVolume = iceLoopAudio.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            iceLoopAudio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        iceLoopAudio.Stop();
        iceLoopAudio.volume = 1f;
        iceLoopAudio.loop = false;
        iceLoopFadeCoroutine = null;
    }
}
