using UnityEngine;
using System.Collections;

public class EnemyAttackEffectController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public GameObject attackEffect; // 特效物体

    [Header("Effect Timing")]
    public float showTime = 2f;
    public float hideTime = 1f;

    private Coroutine effectCoroutine;
    private bool isAttacking = false;

    void Start()
    {
        if (attackEffect != null)
            attackEffect.SetActive(false);
    }

    void Update()
    {
        bool playingAttack =
            animator.GetCurrentAnimatorStateInfo(0).IsName("attack");

        // ▶️ 进入 attack 动画
        if (playingAttack && !isAttacking)
        {
            isAttacking = true;
            effectCoroutine = StartCoroutine(EffectLoop());
        }

        // ⛔ 离开 attack 动画
        if (!playingAttack && isAttacking)
        {
            isAttacking = false;

            if (effectCoroutine != null)
                StopCoroutine(effectCoroutine);

            if (attackEffect != null)
                attackEffect.SetActive(false);
        }
    }

    IEnumerator EffectLoop()
    {
        while (true)
        {
            attackEffect.SetActive(true);
            yield return new WaitForSeconds(showTime);

            attackEffect.SetActive(false);
            yield return new WaitForSeconds(hideTime);
        }
    }
}
