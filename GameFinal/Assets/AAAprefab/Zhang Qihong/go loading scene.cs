using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentSceneLoader : MonoBehaviour
{
    public static CurrentSceneLoader Instance { get; private set; }
    private const string NextSceneName = "NextLevel";
    void Awake() => Instance = this;

    // 由过关检测函数调用
    public void TriggerLevelTransition()
    {
        // 同步加载轻量预加载场景（无UI）
        SceneManager.LoadScene(NextSceneName, LoadSceneMode.Additive);
    }
}
