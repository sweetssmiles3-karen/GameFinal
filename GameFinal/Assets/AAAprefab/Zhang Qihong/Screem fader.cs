using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
// 必须导入 DOTween 后生效
using System;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage; // 全屏 Image（Alpha 初始为 0）
    public float fadeDuration = 0.5f;

    void Awake()
    {
        // 初始状态：完全透明
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    // 淡出（变黑，用于场景切换前）
    public void FadeOut(Action onComplete = null)
    {
        fadeImage.DOKill(); // 终止之前的动画
        fadeImage.DOFade(1, fadeDuration).OnComplete(() => onComplete?.Invoke());
    }

    // 淡入（变透明，用于场景加载后）
    public void FadeIn(Action onComplete = null)
    {
        fadeImage.DOKill();
        fadeImage.DOFade(0, fadeDuration).OnComplete(() => onComplete?.Invoke());
    }
}
