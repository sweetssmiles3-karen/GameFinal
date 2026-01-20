using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentSceneLoader : MonoBehaviour
{
    public string currentSceneName;
    public static CurrentSceneLoader Instance { get; private set; }

    public string NextSceneName;
    void Awake()
    {
        Instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    // 由过关检测函数调用
    public void TriggerLevelTransition()
    {

        // 同步加载轻量预加载场景（无UI）
        SceneManager.LoadScene(NextSceneName, LoadSceneMode.Additive); 
        // 卸载当前场景
        SceneManager.UnloadSceneAsync(currentSceneName);
    }
}
