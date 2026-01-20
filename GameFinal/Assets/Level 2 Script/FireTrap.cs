using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("VFX Objects")]
    public GameObject magicCircle;
    public GameObject explosionVFX;
    public GameObject fireAreaVFX;

    [Header("Damage Zones")]
    public GameObject damageZone;    // 火焰持续伤害区域
    public GameObject explosionZone; // 爆炸伤害区域

    [Header("Audio Sources")]
    public AudioSource warningAudio;
    public AudioSource explosionAudio;
    public AudioSource fireAudio;

    [Header("Audio Clips")]
    public AudioClip warningSound;
    public AudioClip explosionSound;
    public AudioClip fireLoopSound;

    [Header("Timing")]
    public float explosionDelay = 1.5f;
    public float explosionDuration = 0.1f;
    public float fireFadeTime = 1.5f;

    private bool triggered = false;

    private void Start()
    {
        // 一开始隐藏特效和伤害区
        if (magicCircle != null) magicCircle.SetActive(false);
        if (explosionVFX != null) explosionVFX.SetActive(false);
        if (fireAreaVFX != null) fireAreaVFX.SetActive(false);
        if (damageZone != null) damageZone.SetActive(false);
        if (explosionZone != null) explosionZone.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 显示法阵 + 播放预警音
        if (magicCircle != null) magicCircle.SetActive(true);
        if (warningSound != null && warningAudio != null)
            warningAudio.PlayOneShot(warningSound);

        StartCoroutine(TriggerExplosion());
    }

    private IEnumerator TriggerExplosion()
    {
        yield return new WaitForSeconds(explosionDelay);

        // 隐藏法阵
        if (magicCircle != null) magicCircle.SetActive(false);

        // 播放爆炸特效 + 音效
        if (explosionVFX != null) explosionVFX.SetActive(true);
        if (explosionSound != null && explosionAudio != null)
            explosionAudio.PlayOneShot(explosionSound);

        // 启用爆炸区（玩家进入才扣血）
        if (explosionZone != null) explosionZone.SetActive(true);
        yield return new WaitForSeconds(explosionDuration);
        if (explosionZone != null) explosionZone.SetActive(false);

        // 显示火焰区（玩家站在里面持续掉血）
        if (fireAreaVFX != null) fireAreaVFX.SetActive(true);
        if (damageZone != null) damageZone.SetActive(true);

        // 播放火焰循环音
        if (fireLoopSound != null && fireAudio != null)
        {
            fireAudio.clip = fireLoopSound;
            fireAudio.loop = true;
            fireAudio.volume = 1f;
            fireAudio.Play();
        }
    }

    // 停止火焰音效
    public void StopFireLoop()
    {
        if (fireAudio != null && fireAudio.isPlaying)
        {
            StartCoroutine(FadeOutFire(fireFadeTime));
        }
    }

    private IEnumerator FadeOutFire(float duration)
    {
        float startVolume = fireAudio.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            fireAudio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        fireAudio.Stop();
        fireAudio.volume = 1f;
        fireAudio.loop = false;
    }
}
