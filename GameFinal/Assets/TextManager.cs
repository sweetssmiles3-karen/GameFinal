using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class TextManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] text;
    int textnumber = 0;
    int textmax = 0;
    bool textmode = false;
    private void Start()
    {
        textmax = text.Length;
        for (int i = 0; i < text.Length; i++)
        {
            text[i].SetActive(false);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {//go next text
            if (textmode)
            {
                if (textnumber < textmax - 1)
                {
                    text[textnumber].SetActive(false);
                    textnumber++;
                    text[textnumber].SetActive(true);
                }
                else
                {
                    text[textnumber].SetActive(false);
                    textmode = false;
                   // CurrentSceneLoader.Instance.TriggerLevelTransition();
                    StartCoroutine(UnloadAndLoadCoroutine());
                }
            }

        }

    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with " + other.gameObject.name);
       
        textmode = true;
        text[textnumber].SetActive(true);
    }
    private IEnumerator UnloadAndLoadCoroutine()
    {
        // 1. 异步卸载当前场景（等待100%完成）
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        yield return unloadOp;

        // 2. 同步加载新场景（确保旧场景已销毁）
        SceneManager.LoadScene("Loading to level 3");
    }
}