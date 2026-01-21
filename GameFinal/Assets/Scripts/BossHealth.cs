using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossHealth_UI : MonoBehaviour, IDamageable
{
    [Header("阶段血量")]
    public int maxHPPhase1 = 800;
    public int maxHPPhase2 = 800;

    [Header("UI引用")]
    public Image healthFill;   // 血条 Fill
    public Text healthText;    // 血条文字
    public Image healthf2;
    public Text healtht2;
    [Header("Boss状态")]
    public bool isPhase2 = false;
    public int currentHP;
    private bool isDead = false;
    private bool phase2Triggered = false; //  避免重复触发Phase2

    private Animator animator;
    public GameObject dialog;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHP = maxHPPhase1;
        UpdateHealthUI();
        if(dialog!=null)
            dialog.SetActive(false);
    }

    public void TakeDamage(float damagef)
    {
        if (isDead) return;
        int damage = Mathf.RoundToInt(damagef);
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        UpdateHealthUI();

        // Phase1 -> Phase2
        if (!isPhase2 && currentHP <= 0 && !phase2Triggered)
        {
            phase2Triggered = true; //  标记已触发
            StartCoroutine(EnterPhase2());
        }
        // Phase2死亡
        else if (isPhase2 && currentHP <= 0)
        {
            if (!isDead)
            {
               StartCoroutine(Die());
            }
        }
    }

    public void UpdateHealthUI()
    {
        int maxHP = isPhase2 ? maxHPPhase2 : maxHPPhase1;

        if (healthFill != null)
            healthFill.fillAmount = (float)currentHP / maxHP;

        if (healthText != null)
            healthText.text = currentHP + " / " + maxHP;
    }
    public void UpdateHealthUI2() 
    {  
        int maxHP = isPhase2 ? maxHPPhase2 : maxHPPhase1;
        if (healthf2 != null)
            healthf2.fillAmount = (float)currentHP / maxHP;
        if (healtht2 != null)
            healtht2.text = currentHP + " / " + maxHP;
    }

    private IEnumerator EnterPhase2()
    {
        Debug.Log(" Boss进入第二阶段！");
        isPhase2 = true;

        // 播放 PhaseChange / PowerUp 动画
        if (animator != null)
            animator.SetTrigger("PhaseChange");

        // 等待动画播放完成（假设3秒）
        yield return new WaitForSeconds(3f);

        // 血量回满 Phase2
        currentHP = maxHPPhase2;
        UpdateHealthUI();
        StartCoroutine(ShowDia1());
    }

    private IEnumerator Die()
    {
        if (isDead) yield break;

        isDead = true;
        Debug.Log(" Boss死亡");

        // 播放死亡动画
        if (animator != null)
            animator.SetTrigger("dead");
        if(dialog!=null)
            dialog.SetActive(true);

        yield return new WaitForSeconds(2.5f);
        CurrentSceneLoader.Instance.TriggerLevelTransition();
        //yield return UnloadAndLoadCoroutine();

    }
    IEnumerator ShowDia1()
    {
        dialog.SetActive(true);
        yield return new WaitForSeconds(5f);
        dialog.SetActive(false);
    }
    private IEnumerator UnloadAndLoadCoroutine()
    {
        // 1. 异步卸载当前场景（等待100%完成）
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        yield return unloadOp;

        // 2. 同步加载新场景（确保旧场景已销毁）
        SceneManager.LoadScene("END");
    }
}
