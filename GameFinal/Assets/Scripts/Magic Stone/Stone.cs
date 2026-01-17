using UnityEngine;

public class Gem : MonoBehaviour
{
    private GemHintUI gemHintUI;

    void Start()
    {
        gemHintUI = FindObjectOfType<GemHintUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gemHintUI != null)
            {
                gemHintUI.ShowHint();
            }
            SaveManager.Instance.AddStone(1);
            Destroy(gameObject);
        }
    }
}
