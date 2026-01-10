using UnityEngine;
using UnityEngine.UI;

public class StoneDisplay : MonoBehaviour
{
    [SerializeField] private Text stoneText;  // 拖拽UI Text组件到这里
    [SerializeField] private UIManager_stone uiManager;

    void Start()
    {
        // 自动获取UIManager实例（单例模式）
        if (uiManager == null) uiManager = UIManager_stone.Instance;

        // 首次加载显示
        UpdateDisplay();
    }

    void Update()
    {
        // 每帧更新显示（根据需求可优化为事件驱动）
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (stoneText != null && uiManager != null)
        {
            int currentStones = uiManager.LoadStoneCount();
            stoneText.text = $"石头数量：{currentStones}";
        }
    }
}
