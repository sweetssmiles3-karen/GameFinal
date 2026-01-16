using System.Collections;
using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    public Rigidbody targetObject;
    public AudioSource fallSound;

    public float delayTime = 0.1f;

    [Header("Stone Settings")]
    public bool isStone = false;      
    public float stoneSoundDuration = 3f;
    public float fadeOutTime = 1.5f;

    private bool triggered = false;
    private float originalVolume;

    void Start()
    {
        if (fallSound != null)
            originalVolume = fallSound.volume;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(FallAfterDelay());
        }
    }

    IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        targetObject.isKinematic = false;
        targetObject.useGravity = true;

        if (fallSound == null) yield break;

        if (isStone)
            StartCoroutine(PlayStoneSound());
        else
            PlayTreeSound();   
    }

    // 树：只播一次
    void PlayTreeSound()
    {
        fallSound.loop = false;
        fallSound.volume = originalVolume;
        fallSound.Play();
    }

    // 石头：循环 + Fade Out
    IEnumerator PlayStoneSound()
    {
        fallSound.loop = true;
        fallSound.volume = originalVolume;
        fallSound.Play();

        yield return new WaitForSeconds(stoneSoundDuration - fadeOutTime);

        float t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            fallSound.volume = Mathf.Lerp(originalVolume, 0f, t / fadeOutTime);
            yield return null;
        }

        fallSound.Stop();
        fallSound.volume = originalVolume;
        fallSound.loop = false;
    }
}
