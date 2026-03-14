using UnityEngine;

// 플레이어에 부착 - 몬스터 접촉 시 밀리기 / 가드 반격 처리
[RequireComponent(typeof(Rigidbody2D))]
public class MonsterContactHandler : MonoBehaviour
{
    [SerializeField] private MonsterGroup monsterGroup;  // 데이터 조회 용도 - IMonsterVelocityProvider로 접근
    [SerializeField] private string monsterTag = "Monster";
    [SerializeField] private float bossVelocityScale = 0.3f;  // 보스 접촉 시 밀리는 힘 보정

    private IMonsterVelocityProvider _velocityProvider;

    private Rigidbody2D _rb;
    private IGuardable _guardable;
    private PlayerWallState _wallState;
    private bool _isTouchedByMonster;

    // 보스 스폰 시 동적으로 할당 — null이면 일반 라운드
    private BossAIBrain _bossAIBrain;
    private bool _isBossRound;

    // MonsterSpawner가 보스 스폰 직후 호출
    public void SetBoss(BossAIBrain brain)
    {
        _bossAIBrain = brain;
        _isBossRound = brain != null;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _guardable = GetComponent<PlayerGuard>();
        _wallState = GetComponent<PlayerWallState>();
        _velocityProvider = monsterGroup;
    }

    // 외부(PlayerWallState)에서 탈출 완료 시 접촉 상태 초기화
    public void ClearMonsterContact()
    {
        _isTouchedByMonster = false;
    }

    private void FixedUpdate()
    {
        // 벽 탈출 이동 중에는 몬스터 밀기 무시
        if (_wallState != null && _wallState.IsEscaping) return;

        // 보스 넉백(IsBeingPushed) 중 또는 보스가 패턴 실행 중에는 몬스터 밀기 무시
        if (_wallState != null && _wallState.IsBeingPushed) return;
        if (_bossAIBrain != null && _bossAIBrain.State == BossAIBrain.BossState.InPattern) return;

        if (_isTouchedByMonster)
        {
            // 선두 몬스터 속도로 플레이어를 밀기 (IMonsterVelocityProvider 인터페이스로 조회)
            Rigidbody2D[] monsterRbs = _velocityProvider.GetAllRigidbodies();
            if (monsterRbs.Length > 0)
            {
                float velocityX = monsterRbs[0].velocity.x;
                if (_isBossRound) velocityX *= bossVelocityScale;
                _rb.velocity = new Vector2(velocityX, _rb.velocity.y);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(monsterTag)) return;

        if (_guardable.IsGuarding)
            TriggerGuardPush();
        else
            _isTouchedByMonster = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(monsterTag)) return;

        if (_guardable.IsGuarding)
        {
            _isTouchedByMonster = false;
            TriggerGuardPush();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(monsterTag)) return;

        _isTouchedByMonster = false;
        if (!_guardable.IsGuarding)
            _rb.velocity = new Vector2(0f, _rb.velocity.y);
    }

    private void TriggerGuardPush()
    {
        _isTouchedByMonster = false;
        _guardable.OnGuardHit();
    }
}
