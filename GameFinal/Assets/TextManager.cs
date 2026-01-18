using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Space)) {//go next text
            if (textmode) {
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
                }
                }
            
        }
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with " + other.gameObject.name);
        CurrentSceneLoader.Instance.TriggerLevelTransition();
        textmode = true;
        text[textnumber].SetActive(true);
    }

}
