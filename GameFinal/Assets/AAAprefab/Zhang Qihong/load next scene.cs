using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoaderSceneController : MonoBehaviour
{
    private AsyncOperation loadOp;
    public string NextSceneName; // 需在Inspector设置
    public float waitTime = 1.0f;
    private string currentSceneName;
    void Start()
    {
        // 获取当前场景名称
        currentSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(PreloadAndSwitch());
    }

    public IEnumerator PreloadAndSwitch()
    {
        
        // 异步加载目标场景（后台加载）
        loadOp = SceneManager.LoadSceneAsync(NextSceneName);
        loadOp.allowSceneActivation = false;
        yield return new WaitForSeconds (waitTime);
        // 等待资源加载完成（0-0.9阶段）
        while (loadOp.progress < 0.9f)
        {
            yield return null;
        }

        // 最后阶段激活场景
        loadOp.allowSceneActivation = true;
        yield return loadOp;

        // 场景切换完成后卸载当前场景
       SceneManager.UnloadSceneAsync( currentSceneName);
    }
}
