using UnityEngine;
using UnityEngine.UI;

public class BossHealth_UI : MonoBehaviour
{
    [Header("阶段血量")]
    public int maxHPPhase1 = 800;
    public int maxHPPhase2 = 800;

    [Header("UI引用")]
    public Image healthFill;   // 血条的 Fill Image
    public Text healthText;    // 血条文字

    [Header("Boss状态")]
    public bool isPhase2 = false;
    public int currentHP;
    private bool isDead = false;

    void Start()
    {
        currentHP = maxHPPhase1;
        UpdateHealthUI();
    }

    /// <summary>
    /// Boss受到伤害调用
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        UpdateHealthUI();

        if (!isPhase2 && currentHP <= 0)
            EnterPhase2();
        else if (isPhase2 && currentHP <= 0)
            Die();
    }

    /// <summary>
    /// 更新血条UI
    /// </summary>
    public void UpdateHealthUI()
    {
        int maxHP = isPhase2 ? maxHPPhase2 : maxHPPhase1;

        if (healthFill != null)
            healthFill.fillAmount = (float)currentHP / maxHP;  // 🔹 Image Fill

        if (healthText != null)
            healthText.text = currentHP + " / " + maxHP;       // 🔹 普通 Text
    }

    /// <summary>
    /// 进入第二阶段
    /// </summary>
    private void EnterPhase2()
    {
        isPhase2 = true;
        currentHP = maxHPPhase2;
        Debug.Log("💥 Boss进入第二阶段！");
        UpdateHealthUI();

        // 可触发阶段切换动画
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("PhaseChange");
    }

    /// <summary>
    /// Boss死亡
    /// </summary>
    private void Die()
    {
        isDead = true;
        Debug.Log("💀 Boss死亡");


    }
}
