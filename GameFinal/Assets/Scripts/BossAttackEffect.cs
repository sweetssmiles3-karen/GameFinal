using UnityEngine;

public class BossAttackEffect : MonoBehaviour
{
    [Header("Far Attack Effects")]
    public GameObject effectA;   // 特效1（拖）
    public GameObject effectB;   // 特效2（拖）

    void Start()
    {
        // 开局全部关闭
        if (effectA != null) effectA.SetActive(false);
        if (effectB != null) effectB.SetActive(false);
    }

    // 🎯 Animation Event 调用（BossFarAttack）
    public void BossFarAttack()
    {
        if (effectA != null) effectA.SetActive(true);
        if (effectB != null) effectB.SetActive(true);
    }

    // ⛔ Animation Event 调用（FarAttackStop）
    public void FarAttackStop()
    {
        if (effectA != null) effectA.SetActive(false);
        if (effectB != null) effectB.SetActive(false);
    }
}
