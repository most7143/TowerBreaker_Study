using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 스테이지 클리어 또는 게임 오버 시 표시되는 결과 팝업
public class ResultPopup : PopupUI
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button titleButton;

    // 클리어 여부에 따라 텍스트를 다르게 표시하고 팝업을 열기
    public void Show(bool isCleared)
    {
        if (titleText != null)
            titleText.text = isCleared ? "스테이지 클리어!" : "게임 오버";

        Time.timeScale = 0f;
        Open();
    }

    public override void Open()
    {
        base.Open();
        retryButton?.onClick.RemoveAllListeners();
        retryButton?.onClick.AddListener(OnClickRetry);
        titleButton?.onClick.RemoveAllListeners();
        titleButton?.onClick.AddListener(OnClickTitle);
    }


    private void OnClickRetry()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LoadStage(GameManager.Instance.CurrentStageNumber);
    }

    private void OnClickTitle()
    {
        Time.timeScale = 1f;
        GameManager.Instance.GoToTitle();
    }
}
