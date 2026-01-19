using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;  // 暂停界面UI预制体

    private bool isPaused = false;
    public static PauseManager Instance { get; private set; }
    void Start()
    {
        // 初始隐藏暂停界面
        if (pausePanel != null) pausePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 公共方法：显示暂停界面（供UI调用）
    public void ShowPause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;  // 暂停时间
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 公共方法：隐藏暂停界面（供UI调用）
    public void HidePause()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;  // 恢复时间
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 外部触发暂停（如输入检测）
    public void TogglePause()
    {
        if (isPaused) HidePause();
        else ShowPause();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    void OnDestroy()
    {
        Time.timeScale = 1f; // 确保时间恢复正常
    }
}