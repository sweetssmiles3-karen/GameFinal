using UnityEngine;
using System.Collections;

public class TP : MonoBehaviour
{
    public GameObject item;         // 待传送的物体
    public GameObject destination;  // 目标位置
    public ScreenFader screenFader; // 屏幕变黑组件引用

    // 触发传送的公共方法（可通过按钮调用）
    public void StartTeleport()
    {
        StartCoroutine(TeleportSequence());
    }

    // 传送流程协程：淡出→传送→淡入
    private IEnumerator TeleportSequence()
    {
        // 1. 屏幕变黑（淡出）
        yield return StartCoroutine(screenFader.FadeIn());

        yield return new WaitForSeconds(screenFader.fadeDuration); // 使用淡出持续时间

        // 2. 执行传送（瞬移）
        if (item != null && destination != null)
        {
            item.transform.position = destination.transform.position;
        }

        // 3. 屏幕恢复（淡入）
        yield return StartCoroutine(screenFader.FadeOut());
    }
   public void normalTeleport()
    {
        if (item != null && destination != null)
        {
            item.transform.position = destination.transform.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with " + other.gameObject.name);
 
        StartTeleport();
        if(gmlevel2.Instance != null)
        {
            gmlevel2.Instance.togglehint();
        }
    }


}
