using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 运行时状态（内存中临时数据，跨场景保留）
    public int coins = 0;
    public int playerHealth = 100;
    public int currentLevelIndex = 0; // 当前关卡索引（0=第一关）
    public string[] levelNames = { "Level1", "Level2", "Level3" }; // 关卡列表（数据驱动）

    void Awake()
    {
        // 单例逻辑（确保唯一实例，跨场景保留）
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // --- 状态操作方法（仅修改内存数据）---
    public void AddCoins(int amount) => coins += amount;
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetHealth(int health) => playerHealth = Mathf.Clamp(health, 0, 100);
    public void LoadLevel(int index) => SceneManager.LoadScene(levelNames[index]); // 简化版加载

}