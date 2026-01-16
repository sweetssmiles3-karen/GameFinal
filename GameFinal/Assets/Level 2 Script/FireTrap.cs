using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("VFX Objects")]
    public GameObject magicCircle;
    public GameObject explosionVFX;
    public GameObject fireAreaVFX;

    [Header("Damage Zones")]
    public GameObject damageZone;
    public GameObject explosionZone;

    [Header("Audio Sources")]
    public AudioSource warningAudio;   // 预警音
    public AudioSource explosionAudio; // 爆炸音
    public AudioSource fireAudio;      // 火焰循环音

    [Header("Audio Clips")]
    public AudioClip warningSound;
    public AudioClip explosionSound;
    public AudioClip fireLoopSound;

    [Header("Timing")]
    public float explosionDelay = 1.5f;
    public float explosionDuration = 0.1f;
    public float fireFadeTime = 1.5f; // 火焰循环淡出时间

    [Header("Damage")]
    public int explosionDamage = 25;
    public int fireDamagePerTick = 10;
    public float damageInterval = 1f;

    private bool triggered = false;
    private Coroutine fireDamageCoroutine;

    private void Start()
    {
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

        // 显示法阵并播放预警音
        if (magicCircle != null) magicCircle.SetActive(true);
        if (warningSound != null && warningAudio != null)
        {
            warningAudio.PlayOneShot(warningSound);
        }

        StartCoroutine(TriggerExplosion());
    }

    private IEnumerator TriggerExplosion()
    {
        yield return new WaitForSeconds(explosionDelay);

        if (magicCircle != null) magicCircle.SetActive(false);

        // 播放爆炸特效和音效
        if (explosionVFX != null) explosionVFX.SetActive(true);
        if (explosionSound != null && explosionAudio != null)
        {
            explosionAudio.PlayOneShot(explosionSound);
        }

        // 爆炸伤害区启用
        if (explosionZone != null) explosionZone.SetActive(true);
        yield return new WaitForSeconds(explosionDuration);
        if (explosionZone != null) explosionZone.SetActive(false);

        // 显示火焰和持续伤害区
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

    // 火焰持续伤害
    private void OnTriggerStay(Collider other)
    {
        if (!triggered) return;
        if (other.CompareTag("Player") && damageZone.activeSelf && fireDamageCoroutine == null)
        {
            fireDamageCoroutine = StartCoroutine(FireDamageOverTime(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && fireDamageCoroutine != null)
        {
            StopCoroutine(fireDamageCoroutine);
            fireDamageCoroutine = null;
        }
    }

    private IEnumerator FireDamageOverTime(Collider player)
    {
        while (true)
        {
            Debug.Log($"[FireTrap] {player.name} takes fire damage: {fireDamagePerTick}");
            yield return new WaitForSeconds(damageInterval);
        }
    }

    // 火焰循环淡出
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
        fireAudio.volume = 1f; // 恢复原音量
        fireAudio.loop = false;
    }

    // 爆炸伤害事件（Debug）
    private void OnTriggerEnterExplosion(Collider other)
    {
        if (!triggered || explosionZone == null || !explosionZone.activeSelf) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log($"[FireTrap] {other.name} takes explosion damage: {explosionDamage}");
    }
}
