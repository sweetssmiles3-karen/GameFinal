using UnityEngine;
using System.IO;
using System;

// 数据载体：明确需要存档的字段（与GameManager状态对应）
[System.Serializable]
public class SaveData
{
    public int coins;
    public int currentLevelIndex;
    public string lastSaveTime; // 扩展字段：存档时间
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string savePath;

    void Awake()
    {
        // 单例逻辑（可与GameManager共存，均为全局唯一）
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选：若需跨场景调用存档，保留；否则可销毁
            savePath = Path.Combine(Application.persistentDataPath, "save.json");
        }
    }

    // --- 存档：接收GameManager的状态，写入文件 ---
    public void SaveGame(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"存档成功：{savePath}");
        }
        catch (Exception e) { Debug.LogError($"存档失败：{e.Message}"); }
    }

    // --- 读档：从文件读取数据，返回给GameManager ---
    public SaveData LoadGame()
    {
        if (!File.Exists(savePath)) { Debug.Log("无存档文件，返回默认数据"); return new SaveData(); }

        try
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e) { Debug.LogError($"读档失败：{e.Message}"); return new SaveData(); }
    }

    // --- 扩展功能：删除存档 ---
    public void DeleteSave() => File.Delete(savePath);
}
