using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;  // 暂停界面UI预制体

    private bool isPaused = false;

    void Start()
    {
        // 初始隐藏暂停界面
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // 公共方法：显示暂停界面（供UI调用）
    public void ShowPause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;  // 暂停时间
    }

    // 公共方法：隐藏暂停界面（供UI调用）
    public void HidePause()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;  // 恢复时间
    }

    // 外部触发暂停（如输入检测）
    public void TogglePause()
    {
        if (isPaused) HidePause();
        else ShowPause();
    }
}