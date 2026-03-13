using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 전반의 상태(스테이지 잠금/해제)를 관리하는 싱글톤 매니저
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const int StageCount = 3;

    // 각 스테이지의 클리어 여부 (0번=1스테이지, 1번=2스테이지, 2번=3스테이지)
    private bool[] _stageCleared = new bool[StageCount];

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    // 스테이지 입장 가능 여부 (스테이지 번호는 1부터 시작)
    public bool IsStageUnlocked(int stageNumber)
    {
        if (stageNumber == 1) return true;

        int index = stageNumber - 1;
        if (index < 0 || index >= StageCount) return false;

        return _stageCleared[index - 1];
    }

    // 스테이지 클리어 처리 및 다음 스테이지 잠금 해제
    public void ClearStage(int stageNumber)
    {
        int index = stageNumber - 1;
        if (index < 0 || index >= StageCount) return;

        _stageCleared[index] = true;
        SaveProgress();
    }

    public void LoadStage(int stageNumber)
    {
        if (!IsStageUnlocked(stageNumber)) return;

        PlayerPrefs.SetInt("CurrentStage", stageNumber);
        SceneManager.LoadScene("MainScene");
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SaveProgress()
    {
        for (int i = 0; i < StageCount; i++)
            PlayerPrefs.SetInt($"StageCleared_{i}", _stageCleared[i] ? 1 : 0);

        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        for (int i = 0; i < StageCount; i++)
            _stageCleared[i] = PlayerPrefs.GetInt($"StageCleared_{i}", 0) == 1;
    }
}
