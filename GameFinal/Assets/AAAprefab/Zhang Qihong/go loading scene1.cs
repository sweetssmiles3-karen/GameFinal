using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class diesceneloader : MonoBehaviour
{
    public string NextSceneName;
    //创建实例
    public static diesceneloader Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        // 每个场景独立实例（不再跨场景存活）
      //当没有父物体时

        //DontDestroyOnLoad(gameObject);
    }

    // 由过关检测函数调用
    public void TriggerLevelTransition()
    {

        StartCoroutine(UnloadAndLoadCoroutine());
    }

    private IEnumerator UnloadAndLoadCoroutine()
    {
        // 1. 异步卸载当前场景（等待100%完成）
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        yield return unloadOp;

        // 2. 同步加载新场景（确保旧场景已销毁）
        SceneManager.LoadScene(NextSceneName);
    }
}