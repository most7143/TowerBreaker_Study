using UnityEngine;

// 타이틀 씬의 메인 버튼 3개 처리 (게임 시작, 아이템, 종료)
public class TitleSceneUI : MonoBehaviour
{
    [SerializeField] private StageSelectPopup stageSelectPopup;

    public void OnClickStart()
    {
        stageSelectPopup.Open();
    }

    public void OnClickItem()
    {
        // TODO: 아이템 팝업 열기
        Debug.Log("아이템 버튼 클릭");
    }

    public void OnClickQuit()
    {
        GameManager.Instance.QuitGame();
    }
}
