using UnityEngine;

public class gamemanager : MonoBehaviour
{
public int killCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        SaveManager.Instance.stoneCount = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (SaveManager.Instance.stoneCount > 8)
        {
            CurrentSceneLoader.Instance.TriggerLevelTransition();
        }
    }
}
