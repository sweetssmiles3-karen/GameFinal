using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoaderSceneController : MonoBehaviour
{
    private AsyncOperation loadOp;
    private const string NextSceneName = "NextLevel"; // 需在Inspector设置

    void Start()
    {
        StartCoroutine(PreloadAndSwitch());
    }

    private IEnumerator PreloadAndSwitch()
    {
        // 异步加载目标场景（后台加载）
        loadOp = SceneManager.LoadSceneAsync(NextSceneName);
        loadOp.allowSceneActivation = false;

        // 等待资源加载完成（0-0.9阶段）
        while (loadOp.progress < 0.9f)
        {
            yield return null;
        }

        // 最后阶段激活场景
        loadOp.allowSceneActivation = true;
        yield return loadOp;

        // 场景切换完成后卸载预加载场景
        SceneManager.UnloadSceneAsync("LoaderScene");
    }
}
