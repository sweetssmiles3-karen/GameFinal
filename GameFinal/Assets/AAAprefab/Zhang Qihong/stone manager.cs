using UnityEngine;
using System.IO;
using System;

[Serializable]
public class GameData
{
    public int stoneCount; // 石头数量
}

public class UIManager_stone: MonoBehaviour
{
    // 单例实例（跨场景存活）
    public static UIManager_stone Instance { get; private set; }

    // 数据文件路径（使用Unity持久化路径，跨平台兼容）
    private string dataFilePath;
    // 数据文件名（可自定义）
    private const string DATA_FILENAME = "StoneData.json";
    // 场景索引键（PlayerPrefs存储）
    private const string SCENE_INDEX_KEY = "CurrentSceneIndex";

    void Awake()
    {
        // 单例初始化（确保跨场景唯一）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景不销毁
            InitializeDataFile(); // 初始化数据文件
        }
        else
        {
            Destroy(gameObject); // 避免重复实例
        }
    }

    /// <summary>
    /// 初始化数据文件（若不存在则创建，默认石头数量0）
    /// </summary>
    private void InitializeDataFile()
    {
        dataFilePath = Path.Combine(Application.persistentDataPath, DATA_FILENAME);
        if (!File.Exists(dataFilePath))
        {
            GameData defaultData = new GameData { stoneCount = 0 };
            SaveStoneData(defaultData); // 创建并写入默认数据
            Debug.Log($"数据文件已创建：{dataFilePath}");
        }
    }

    /// <summary>
    /// 读取石头数量（启动时调用）
    /// </summary>
    /// <returns>当前石头数量</returns>
    public int LoadStoneCount()
    {
        if (File.Exists(dataFilePath))
        {
            GameData data = JsonUtility.FromJson<GameData>(File.ReadAllText(dataFilePath));
            return data.stoneCount;
        }
        else
        {
            Debug.LogError($"数据文件不存在：{dataFilePath}，将创建新文件");
            InitializeDataFile();
            return 0;
        }
    }

    /// <summary>
    /// 保存石头数量（修改后调用）
    /// </summary>
    /// <param name="count">新的石头数量</param>
    public void SaveStoneCount(int count)
    {
        if (File.Exists(dataFilePath))
        {
            GameData data = new GameData { stoneCount = count };
            SaveStoneData(data);
            Debug.Log($"石头数量已保存：{count}");
        }
        else
        {
            Debug.LogError($"数据文件不存在，无法保存：{dataFilePath}");
        }
    }

    /// <summary>
    /// 删除石头数据文件（备用功能）
    /// </summary>
    public void DeleteStoneData()
    {
        if (File.Exists(dataFilePath))
        {
            File.Delete(dataFilePath);
            Debug.Log($"石头数据文件已删除：{dataFilePath}");
        }
        else
        {
            Debug.LogWarning($"数据文件不存在，无需删除：{dataFilePath}");
        }
    }

    /// <summary>
    /// 获取当前场景索引（从PlayerPrefs读取）
    /// </summary>
    /// <returns>当前场景索引</returns>
    public int GetCurrentSceneIndex()
    {
        return PlayerPrefs.GetInt(SCENE_INDEX_KEY, 0); // 默认返回0（主菜单）
    }

    /// <summary>
    /// 设置当前场景索引（切换场景后调用）
    /// </summary>
    /// <param name="index">新场景索引</param>
    public void SetCurrentSceneIndex(int index)
    {
        PlayerPrefs.SetInt(SCENE_INDEX_KEY, index);
        PlayerPrefs.Save(); // 立即保存到本地
        Debug.Log($"场景索引已保存：{index}");
    }

    /// <summary>
    /// 私有方法：保存石头数据到JSON文件
    /// </summary>
    /// <param name="data">要保存的游戏数据</param>
    private void SaveStoneData(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true); // 格式化JSON（易读）
            File.WriteAllText(dataFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存数据失败：{e.Message}");
        }
    }
}

