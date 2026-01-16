using UnityEngine;
using System.Collections;

public class HeadUIFlash : MonoBehaviour
{
    public GameObject exclamationUI; // 头顶UI
    public float displayTime = 2f;   // 闪烁总时长

    // 允许外部调用显示UI
    public void FlashUI()
    {
        if (exclamationUI != null)
        {
            exclamationUI.SetActive(true);
            StartCoroutine(HideUIAfterDelay());
        }
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        if (exclamationUI != null)
            exclamationUI.SetActive(false);
    }
}
