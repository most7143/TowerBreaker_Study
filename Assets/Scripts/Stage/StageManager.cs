using UnityEngine;

// 스테이지의 라운드 진행을 관리 - 몬스터가 전멸하면 다음 라운드로 진행
public class StageManager : MonoBehaviour
{
    [SerializeField] private MonsterSpawner spawner;
    [SerializeField] private MonsterGroup monsterGroup;
    [SerializeField] private ResultPopup resultPopup;
    [SerializeField] private UpgradePopup upgradePopup;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private WallCollisionReporter wallCollisionReporter;
    [SerializeField] private PlayerUpgradeState upgradeState;

    // Inspector에서 직접 연결하는 fallback용 (GameManager에서 로드 실패 시 사용)
    [SerializeField] private StageData fallbackStageData;

    private StageData _stageData;
    private int _currentRound = 0;
    private bool _isBossRound = false;

    private void Start()
    {
        // GameManager에서 현재 스테이지 데이터를 로드
        _stageData = GameManager.Instance != null
            ? GameManager.Instance.GetCurrentStageData()
            : null;

        // GameManager 로드 실패 시 fallback 사용
        if (_stageData == null)
        {
            _stageData = fallbackStageData;
            Debug.LogWarning("GameManager에서 StageData 로드 실패. fallbackStageData를 사용합니다.");
        }

        if (_stageData == null)
        {
            Debug.LogError("StageData가 없습니다. StageManager 또는 GameManager를 확인하세요.");
            return;
        }

        // StageData의 monsterPrefab으로 MonsterPool 초기화
        if (_stageData.MonsterPrefab != null)
            MonsterPool.Instance.InitializeWithPrefab(_stageData.MonsterPrefab);
        else
            Debug.LogWarning("StageData에 MonsterPrefab이 설정되지 않았습니다.");

        // 이벤트 구독
        monsterGroup.OnRoundCleared += HandleRoundCleared;
        monsterGroup.OnWallHit += HandleWallHit;
        playerStats.OnPlayerDead += HandlePlayerDead;

        // MonsterGroup에 PlayerAttack, WallCollisionReporter 이벤트 연결
        monsterGroup.SubscribeToAttack(playerAttack);
        monsterGroup.SubscribeToWallReporter(wallCollisionReporter);

        StartRound(_currentRound);
    }

    private void OnDestroy()
    {
        monsterGroup.OnRoundCleared -= HandleRoundCleared;
        monsterGroup.OnWallHit -= HandleWallHit;
        playerStats.OnPlayerDead -= HandlePlayerDead;
        monsterGroup.UnsubscribeFromAttack(playerAttack);
        monsterGroup.UnsubscribeFromWallReporter(wallCollisionReporter);
    }

    private void StartRound(int roundIndex)
    {
        RoundData round = _stageData.GetRound(roundIndex);

        // 플레이어 위치 초기화 (벽 x 위치 기준, y는 중력으로 자연 착지)
        PlayerWallState wallState = playerStats.GetComponent<PlayerWallState>();
        if (wallState != null && wallState.WallTransform != null)
            playerStats.ResetPositionX(wallState.WallTransform.position.x);

        // 플레이어 애니메이션 초기화
        PlayerAnimator playerAnimator = playerStats.GetComponent<PlayerAnimator>();
        playerAnimator?.ResetToIdle();

        monsterGroup.ResetState();
        spawner.SpawnRound(round);
        Debug.Log($"[Stage {GameManager.Instance?.CurrentStageNumber}] Round {roundIndex + 1} 시작 - 몬스터 {round.monsterCount}마리 / HP {round.monsterHp}");
    }

    private void HandleRoundCleared()
    {
        // 보스 라운드가 끝나면 바로 클리어
        if (_isBossRound)
        {
            OnStageCleared();
            return;
        }

        _currentRound++;

        bool allRoundsDone = _currentRound >= _stageData.RoundCount;

        if (allRoundsDone)
        {
            // 일반 라운드 전부 완료 — 보스 또는 클리어 전에 강화 팝업 표시
            if (upgradePopup != null)
            {
                upgradePopup.OnClosed += OnUpgradePopupClosedForBoss;
                upgradePopup.Show(upgradeState, playerStats);
            }
            else
            {
                ProceedAfterAllRounds();
            }
            return;
        }

        // 일반 라운드 사이 강화 팝업
        if (upgradePopup != null)
        {
            upgradePopup.OnClosed += OnUpgradePopupClosed;
            upgradePopup.Show(upgradeState, playerStats);
        }
        else
        {
            StartRound(_currentRound);
        }
    }

    private void OnUpgradePopupClosed()
    {
        upgradePopup.OnClosed -= OnUpgradePopupClosed;
        StartRound(_currentRound);
    }

    private void OnUpgradePopupClosedForBoss()
    {
        upgradePopup.OnClosed -= OnUpgradePopupClosedForBoss;
        ProceedAfterAllRounds();
    }

    private void ProceedAfterAllRounds()
    {
        if (_stageData.BossPrefab != null)
            StartBossRound();
        else
            OnStageCleared();
    }

    private void StartBossRound()
    {
        _isBossRound = true;

        // 플레이어 위치 초기화
        PlayerWallState wallState = playerStats.GetComponent<PlayerWallState>();
        if (wallState != null && wallState.WallTransform != null)
            playerStats.ResetPositionX(wallState.WallTransform.position.x);

        PlayerAnimator playerAnimator = playerStats.GetComponent<PlayerAnimator>();
        playerAnimator?.ResetToIdle();

        monsterGroup.ResetState();
        spawner.SpawnBoss(_stageData.BossPrefab, _stageData.BossRound);
        Debug.Log($"[Stage {GameManager.Instance?.CurrentStageNumber}] 보스 라운드 시작");
    }

    private void HandleWallHit()
    {
        playerStats.OnWallHit(1);
    }

    private void OnStageCleared()
    {
        Debug.Log("스테이지 클리어!");

        if (GameManager.Instance != null)
            GameManager.Instance.ClearStage(GameManager.Instance.CurrentStageNumber);

        ShowResult(true);
    }

    private void HandlePlayerDead()
    {
        Debug.Log("게임 오버!");
        monsterGroup.HideAllMonsters();
        ShowResult(false);
    }

    private void ShowResult(bool isCleared)
    {
        if (resultPopup != null)
            resultPopup.Show(isCleared);
        else
            Debug.LogWarning("StageManager: ResultPopup이 연결되지 않았습니다.");
    }
}
