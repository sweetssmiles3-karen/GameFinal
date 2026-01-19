using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public EnermyHealth enemyHealth; // Inspector 拖敌人脚本
    public Image fillImage;          // Inspector 拖血条前景Image

    void Start()
    {
        if (enemyHealth == null)
            Debug.LogError(" EnemyHealthBar 找不到 EnermyHealth");

        if (fillImage == null)
            Debug.LogError(" EnemyHealthBar 找不到 Fill Image");
    }

    void Update()
    {
        if (enemyHealth == null || fillImage == null) return;

        // 更新血条填充比例
        float fillAmount = Mathf.Clamp01((float)enemyHealth.CurrentHP / enemyHealth.MaxHP);
        fillImage.fillAmount = fillAmount;

        // 血条在敌人死亡时隐藏
        if (enemyHealth.CurrentHP <= 0)
            fillImage.transform.parent.gameObject.SetActive(false);
    }
}
