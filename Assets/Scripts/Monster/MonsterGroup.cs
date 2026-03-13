using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 몬스터 기차 대열을 관리 - 대열이 플레이어를 밀고, 나머지는 앞 몬스터에 붙어서 따라옴
// 충돌 처리: Rigidbody2D + Collider2D 기반 (플레이어의 WallCollisionReporter가 충돌 이벤트 전달)
public class MonsterGroup : MonoBehaviour
{
    [SerializeField] private float monsterSpacing = 1f;  // 몬스터 간 간격
    [SerializeField] private int wallDamage = 10;
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform player;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private PlayerAttack playerAttack;

    private readonly List<MonsterMover> _monsters = new List<MonsterMover>();
    private PlayerStats _playerStats;
    private bool _stoppedByWall;

    private void Start()
    {
        _playerStats = player.GetComponent<PlayerStats>();
        playerAttack.OnAttackStarted += ResumeAllMonsters;
    }

    private void OnDestroy()
    {
        playerAttack.OnAttackStarted -= ResumeAllMonsters;
    }

    // 물리 이동은 FixedUpdate에서 처리
    private void FixedUpdate()
    {
        if (_monsters.Count == 0) return;
        if (_stoppedByWall) return;

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

    // 플레이어의 Collider2D가 왼쪽 벽 Collider2D와 충돌할 때 호출됨
    // 플레이어 GameObject에 WallCollisionReporter 컴포넌트를 달아 이벤트를 전달받음
    public void OnPlayerHitWall()
    {
        if (_stoppedByWall) return;

        _playerStats.TakeDamage(wallDamage);
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

    public void AddMonster(MonsterMover monster)
    {
        _monsters.Add(monster);
    }

    public void RemoveMonster(MonsterMover monster)
    {
        _monsters.Remove(monster);

        if (_monsters.Count == 0)
        {
            if (stageManager == null)
                Debug.LogError("MonsterGroup: StageManager가 연결되지 않았습니다. Inspector에서 할당해주세요.", this);
            else
                stageManager.OnRoundCleared();
        }
    }

    // 다음 라운드 시작 전 상태 초기화
    public void ResetState()
    {
        _monsters.Clear();
        _stoppedByWall = false;
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
