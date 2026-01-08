using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    // 淡出协程（返回 IEnumerator）
    public IEnumerator FadeOut()
    {
        fadeImage.DOKill();
        yield return fadeImage.DOFade(1, fadeDuration).WaitForCompletion();
    }

    // 淡入协程
    public IEnumerator FadeIn()
    {
        fadeImage.DOKill();
        yield return fadeImage.DOFade(0, fadeDuration).WaitForCompletion();
    }
}
