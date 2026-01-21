using UnityEngine;
using UnityEngine.UI;

public class stoneUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Text stoneUI1;


    // Update is called once per frame
    void Update()
    {
        // 每帧更新UI显示的stone数量
        stoneUI1.text = SaveManager.Instance.stoneCount.ToString() +"/"+ gamemanager.Instance.maxstone.ToString();
    }
}
