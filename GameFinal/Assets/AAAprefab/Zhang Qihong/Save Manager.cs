using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    // 直接暴露的存档数据（启动时自动加载，修改后自动保存）
    public int stoneCount;
    public int levelCode;

    private string savePath;
    public static SaveManager Instance { get; private set; }
    void Awake()
    {
        // 初始化路径&单例
        savePath = Path.Combine(Application.persistentDataPath, "save.dat");
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 启动时自动加载存档（无存档则新建默认值）
        Load();
    }

    // 存储操作（直接修改public变量后自动保存）
    public void AddStone(int num) { stoneCount += num; Save(); }
    public void UseStone(int num)
    {
        if (stoneCount >= num)
        {
            stoneCount -= num;
            Save();
        }
        else Debug.LogWarning("Stone不足！");
    }
    public void SetLevel(int code) { levelCode = code; Save(); }

    // 读档逻辑（异常容错）
    private void Load()
    {
        if (File.Exists(savePath))
        {
            try { stoneCount = int.Parse(File.ReadAllText(savePath).Split(',')[0]); }
            catch { ResetDefaults(); }
        }
        else ResetDefaults();
    }

    // 默认值重置（异常时触发）
    private void ResetDefaults()
    {
        stoneCount = 0;
        levelCode = 0;
        Save();
    }

    // 存档逻辑（直接覆盖文件）
    private void Save() => File.WriteAllText(savePath, $"{stoneCount},{levelCode}");
}