using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{

    public int maxstone;
    public GameObject dia1;
    public GameObject dia2;
    public GameObject dia3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        bool d1isplayed = false;
        bool d2isplayed = false;
        bool d3isplayed = false;
        bool loading = false;
    public AudioSource passLevel;
    public static gamemanager Instance { get; private set; }
    void Start()
    {
        dia1.SetActive(false);
        dia2.SetActive(false);
        dia3.SetActive(false);
        Instance = this;    
        SaveManager.Instance.stoneCount = 0;

    }
    // Update is called once per frame
    void Update()
    {
        if(SaveManager.Instance.stoneCount == 0&&dia1!=null&& !d1isplayed )
        {

            d1isplayed = true;
            StartCoroutine(ShowDia1());
        }
        if (SaveManager.Instance.stoneCount == 7&&dia1!=null&& !d2isplayed )
        {
            d2isplayed = true;
           StartCoroutine(ShowDia2() );
        }
        if (SaveManager.Instance.stoneCount == 15 && dia2!=null && !d3isplayed)
        {
            d3isplayed = true;
            StartCoroutine (ShowDia3() );
        }

        if (SaveManager.Instance.stoneCount == maxstone&&!loading)
        {
            loading = true;
            passLevel.Play();
            StartCoroutine(NextLevel());
        }
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(2f);
      //  CurrentSceneLoader.Instance.TriggerLevelTransition();
      StartCoroutine(UnloadAndLoadCoroutine());
    }
    private IEnumerator UnloadAndLoadCoroutine()
    {
        // 1. 异步卸载当前场景（等待100%完成）
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        yield return unloadOp;

        // 2. 同步加载新场景（确保旧场景已销毁）
        SceneManager.LoadScene("Loading to level 2");
    }
    IEnumerator ShowDia1()
    {
        dia1.SetActive(true);
        Debug.Log("play d1");
        yield return new WaitForSeconds(15f);
        dia1.SetActive(false);
    }
    IEnumerator ShowDia2()
    {
        dia2.SetActive(true);
        Debug.Log("play d2");
        yield return new WaitForSeconds(5f);
        dia2.SetActive(false);
    }
    IEnumerator ShowDia3()
    {
        dia3.SetActive(true);
        yield return new WaitForSeconds(5f);
        dia3.SetActive(false);
    }
}
