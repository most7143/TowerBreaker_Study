using UnityEngine;
using UnityEngine.UI;

// 스테이지 선택 팝업 - 스테이지 잠금/해제 상태를 반영해서 버튼 표시
public class StageSelectPopup : MonoBehaviour
{
    [SerializeField] private Button[] stageButtons; // 스테이지 수만큼 버튼 (Inspector에서 연결)

    public void Open()
    {
        gameObject.SetActive(true);
        RefreshButtons();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNumber = i + 1;
            stageButtons[i].interactable = GameManager.Instance.IsStageUnlocked(stageNumber);
        }
    }

    // 스테이지 버튼 클릭 시 호출 (Button의 OnClick에서 연결)
    public void OnClickStage(int stageNumber)
    {
        GameManager.Instance.LoadStage(stageNumber);
    }
}
