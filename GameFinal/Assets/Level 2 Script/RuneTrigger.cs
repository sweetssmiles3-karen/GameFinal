using UnityEngine;

public class RuneTrigger : MonoBehaviour
{
    [SerializeField] private Runestone_Controller runeStone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            runeStone.ToggleRuneStone(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            runeStone.ToggleRuneStone(false);
        }
    }
}
