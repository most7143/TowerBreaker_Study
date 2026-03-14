using System.Collections.Generic;
using UnityEngine;

// 보스 AI의 FSM 두뇌
// 같은 오브젝트에 부착된 IBossPattern 컴포넌트를 자동 수집해 관리
// 패턴 선택 우선순위: IsReady → RequiresCloseRange(플레이어 근접 시) → 리스트 앞쪽 순서
public class BossAIBrain : MonoBehaviour
{
    // ─── FSM 상태 ───────────────────────────────────────────────
    public enum BossState { Idle, InPattern, Dead }

    [Header("AI 설정")]
    [SerializeField] private float closeRangeDistance = 3f;   // 근접 우선 패턴 판정 거리
    [SerializeField] private float idleCheckInterval = 0.2f;  // Idle 상태에서 패턴 탐색 주기

    [Header("디버그 (읽기 전용)")]
    [SerializeField] private BossState _currentState = BossState.Idle;
    public BossState State => _currentState;

    private Transform _player;
    private MonsterGroup _monsterGroup;
    private float _nextCheckTime;
    private IBossPattern _activePattern;

    // IBossPattern 인터페이스로만 참조 — 구체 패턴 클래스를 모른다
    private readonly List<IBossPattern> _patterns = new List<IBossPattern>();

    // ─── 초기화 ─────────────────────────────────────────────────
    private void Awake()
    {
        // 같은 오브젝트에 붙은 모든 IBossPattern 컴포넌트를 수집
        // GetComponents<Interface>는 직접 지원 안 되므로 MonoBehaviour 경유
        foreach (var mb in GetComponents<MonoBehaviour>())
        {
            if (mb is IBossPattern pattern)
                _patterns.Add(pattern);
        }
    }

    // BossStats.Initialize()에서 스폰 시 호출 — 플레이어를 태그로 자동 탐색
    public void Initialize(MonsterGroup monsterGroup)
    {
        _monsterGroup = monsterGroup;

        GameObject playerGo = GameObject.FindWithTag("Player");
        if (playerGo == null)
        {
            Debug.LogError("BossAIBrain: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
            return;
        }

        _player = playerGo.transform;
        _currentState = BossState.Idle;
        _nextCheckTime = 0f;

        foreach (var p in _patterns)
            p.ResetState();
    }

    // ─── FSM 업데이트 ────────────────────────────────────────────
    private void Update()
    {
        switch (State)
        {
            case BossState.Idle:    UpdateIdle(); break;
            case BossState.InPattern: break;  // 패턴 코루틴이 스스로 완료 콜백을 보냄
            case BossState.Dead:    break;
        }
    }

    private void UpdateIdle()
    {
        if (Time.time < _nextCheckTime) return;
        _nextCheckTime = Time.time + idleCheckInterval;

        IBossPattern selected = SelectPattern();
        if (selected == null) return;

        _activePattern = selected;
        _currentState = BossState.InPattern;

        if (selected.StopsMovement)
            _monsterGroup?.StopAllMonstersByPattern();

        selected.Execute(transform, _player, OnPatternComplete);
    }

    // ─── 패턴 선택 ──────────────────────────────────────────────
    // 1. IsReady인 패턴만 후보
    // 2. 플레이어가 근접 범위 내라면 RequiresCloseRange 패턴 우선
    // 3. 동순위면 리스트 앞쪽(등록 순서) 우선
    private IBossPattern SelectPattern()
    {
        bool playerIsClose = _player != null &&
            Vector2.Distance(transform.position, _player.position) <= closeRangeDistance;

        IBossPattern bestClose = null;
        IBossPattern bestAny   = null;

        foreach (var pattern in _patterns)
        {
            if (!pattern.IsReady) continue;

            if (pattern.RequiresCloseRange && playerIsClose && bestClose == null)
                bestClose = pattern;

            if (bestAny == null)
                bestAny = pattern;
        }

        return bestClose ?? bestAny;
    }

    // ─── 상태 전환 콜백 ─────────────────────────────────────────
    private void OnPatternComplete()
    {
        if (State == BossState.Dead) return;

        if (_activePattern != null && _activePattern.StopsMovement)
            _monsterGroup?.ResumeAllMonstersByPattern();

        _activePattern = null;
        _currentState = BossState.Idle;
    }

    public void OnBossDead()
    {
        _currentState = BossState.Dead;
        enabled = false;
    }
}
