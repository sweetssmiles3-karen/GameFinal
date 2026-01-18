using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] public float fadeDuration = 0.5f;

    void Start()
    {
        // 初始化为透明
        SetAlpha(0);
    }

    // 直接作为协程使用
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(AnimateAlpha(0));
    }

    // 直接作为协程使用
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(AnimateAlpha(1));
    }

    private IEnumerator AnimateAlpha(float target)
    {
        float current = targetImage.color.a;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            SetAlpha(Mathf.Lerp(current, target, t));
            yield return null; // 关键：保持协程活性
        }
        SetAlpha(target);
    }

    private void SetAlpha(float alpha)
    {
        Color color = targetImage.color;
        color.a = alpha;
        targetImage.color = color;
    }
}