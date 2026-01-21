using UnityEngine;

public class l3gm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject boss;


    // Update is called once per frame
    void Update()
    {
        if (boss == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("END");
        }
    }
}
