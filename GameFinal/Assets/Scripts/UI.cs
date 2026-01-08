using UnityEngine;
using System.Collections;

public class HeadUIFlash : MonoBehaviour
{
    public GameObject exclamationUI; // 头顶UI
    public float displayTime = 2f;   // 闪烁总时长

    void Start()
    {
        if (exclamationUI != null)
        {
            exclamationUI.SetActive(true); // 开局显示
            StartCoroutine(HideUIAfterDelay());
        }
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);

        if (exclamationUI != null)
            exclamationUI.SetActive(false); // 消失
    }
}
