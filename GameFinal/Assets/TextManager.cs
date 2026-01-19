using UnityEngine;

public class DialogueImagesTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;      // optional parent container
    public GameObject[] dialogues;         // Dialogue1 until Dialogue8

    private int index = 0;
    private bool inDialogue = false;
    private bool played = false;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        for (int i = 0; i < dialogues.Length; i++)
            dialogues[i].SetActive(false);
    }

    void Update()
    {
        if (!inDialogue) return;

        if (Input.GetKeyDown(KeyCode.Space))
            Next();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (played) return; // remove this if you want it repeatable

        played = true;
        StartDialogue();
    }

    void StartDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        inDialogue = true;
        index = 0;
        dialogues[index].SetActive(true);
    }

    void Next()
    {
        dialogues[index].SetActive(false);
        index++;

        if (index >= dialogues.Length)
        {
            inDialogue = false;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);

            // Optional: load next level AFTER dialogue finishes
            // CurrentSceneLoader.Instance.TriggerLevelTransition();
        }
        else
        {
            dialogues[index].SetActive(true);
        }
    }
}
