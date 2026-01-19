using UnityEngine;
using TMPro;
using System.Collections;

public class GemHintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public float showTime = 1.5f;

    void Start()
    {
        hintText.gameObject.SetActive(false);
    }

    public void ShowHint()
    {
        StopAllCoroutines();
        StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        hintText.gameObject.SetActive(true);
        hintText.text = "¼ñµ½±¦Ê¯ +1";
        yield return new WaitForSeconds(showTime);
        hintText.gameObject.SetActive(false);
    }
}
