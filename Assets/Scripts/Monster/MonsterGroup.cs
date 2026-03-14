using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 몬스터 기차 대열을 관리 - 대열이 플레이어를 밀고, 나머지는 앞 몬스터에 붙어서 따라옴
// 충돌 처리: Rigidbody2D + Collider2D 기반 (플레이어의 WallCollisionReporter가 충돌 이벤트 전달)
public class MonsterGroup : MonoBehaviour, IMonsterVelocityProvider
{
    [SerializeField] private float monsterSpacing = 1f;  // 몬스터 간 간격
    [SerializeField] private Transform player;

    // 라운드 전멸 시 발행 - StageManager가 구독
    public event Action OnRoundCleared;

    // 플레이어가 벽에 닿았을 때 발행 - PlayerStats가 구독해 데미지 처리
    public event Action OnWallHit;

    private readonly List<MonsterMover> _monsters = new List<MonsterMover>();
    private bool _stoppedByWall;
    private bool _stoppedByPattern;

    // 외부(StageManager)에서 PlayerAttack 이벤트 연결
    public void SubscribeToAttack(PlayerAttack playerAttack)
    {
        playerAttack.OnAttackStarted += ResumeAllMonsters;
    }

    public void UnsubscribeFromAttack(PlayerAttack playerAttack)
    {
        playerAttack.OnAttackStarted -= ResumeAllMonsters;
    }

    // 외부(StageManager)에서 WallCollisionReporter 이벤트 연결
    public void SubscribeToWallReporter(WallCollisionReporter reporter)
    {
        reporter.OnHitWallEvent += OnPlayerHitWall;
    }

    public void UnsubscribeFromWallReporter(WallCollisionReporter reporter)
    {
        reporter.OnHitWallEvent -= OnPlayerHitWall;
    }

    // 물리 이동은 FixedUpdate에서 처리
    private void FixedUpdate()
    {
        if (_monsters.Count == 0) return;
        if (_stoppedByWall || _stoppedByPattern) return;

        foreach (var m in _monsters)
            m.Resume();

        // 선두 몬스터: 플레이어 위치를 목표로
        _monsters[0].MoveTo(player.position);

        // 나머지 몬스터: 앞 몬스터 오른쪽에 붙어서 따라옴
        for (int i = 1; i < _monsters.Count; i++)
        {
            Vector3 followTarget = _monsters[i - 1].transform.position
                + new Vector3(monsterSpacing, 0f, 0f);
            _monsters[i].MoveTo(followTarget);
        }
    }

    // 플레이어가 벽에 충돌했을 때 호출됨 (WallCollisionReporter → 이벤트 구독)
    public void OnPlayerHitWall()
    {
        if (_stoppedByWall) return;

        OnWallHit?.Invoke();
        StopAllMonsters();
    }

    private void StopAllMonsters()
    {
        _stoppedByWall = true;
        foreach (var m in _monsters)
            m.Stop();
    }

    public void ResumeAllMonsters()
    {
        if (!_stoppedByWall) return;
        _stoppedByWall = false;
        foreach (var m in _monsters)
            m.Resume();
    }

    // 보스 패턴 실행 중 이동 중단 — 벽 충돌 로직과 독립적으로 동작
    public void StopAllMonstersByPattern()
    {
        _stoppedByPattern = true;
        foreach (var m in _monsters)
            m.Stop();
    }

    public void ResumeAllMonstersByPattern()
    {
        _stoppedByPattern = false;
        foreach (var m in _monsters)
            m.Resume();
    }

    public void AddMonster(MonsterMover monster)
    {
        _monsters.Add(monster);
    }

    public void RemoveMonster(MonsterMover monster)
    {
        _monsters.Remove(monster);

        if (_monsters.Count == 0)
            OnRoundCleared?.Invoke();
    }

    // 플레이어 사망 시 모든 몬스터 비활성화
    public void HideAllMonsters()
    {
        foreach (var m in _monsters)
            m.gameObject.SetActive(false);
        _monsters.Clear();
    }

    // 다음 라운드 시작 전 상태 초기화
    public void ResetState()
    {
        _monsters.Clear();
        _stoppedByWall = false;
        _stoppedByPattern = false;
    }

    public Rigidbody2D[] GetAllRigidbodies()
    {
        return _monsters
            .Select(m => m.GetComponent<Rigidbody2D>())
            .Where(rb => rb != null)
            .ToArray();
    }

    public MonsterMover[] GetAllMovers()
    {
        return _monsters.ToArray();
    }
}
