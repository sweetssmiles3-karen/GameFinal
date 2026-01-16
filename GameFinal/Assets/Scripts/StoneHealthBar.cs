using UnityEngine;
using UnityEngine.UI;

public class StoneHealthBar : MonoBehaviour
{
    public StoneHealth stoneHealth;  // 拖 StoneHealth
    public Image fillImage;          // 血条前景 Image

    void Start()
    {
        if (stoneHealth == null)
            Debug.LogError("❌ StoneHealthBar 找不到 StoneHealth");

        if (fillImage == null)
            Debug.LogError("❌ StoneHealthBar 找不到 Fill Image");
    }

    void Update()
    {
        if (stoneHealth == null || fillImage == null) return;

        float fillAmount = Mathf.Clamp01(
            (float)stoneHealth.CurrentHP / stoneHealth.MaxHP
        );

        fillImage.fillAmount = fillAmount;

        // Stone 死亡时隐藏血条
        if (stoneHealth.CurrentHP <= 0)
        {
            fillImage.transform.parent.gameObject.SetActive(false);
        }
    }
}
