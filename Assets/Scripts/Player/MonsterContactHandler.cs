using UnityEngine;

// 플레이어에 부착 - 몬스터 접촉 시 밀리기 / 가드 반격 처리
[RequireComponent(typeof(Rigidbody2D))]
public class MonsterContactHandler : MonoBehaviour
{
    [SerializeField] private MonsterGroup monsterGroup;
    [SerializeField] private string monsterTag = "Monster";

    private Rigidbody2D _rb;
    private IGuardable _guardable;
    private PlayerWallState _wallState;
    private bool _isTouchedByMonster;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _guardable = GetComponent<PlayerGuard>();
        _wallState = GetComponent<PlayerWallState>();
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

        if (_isTouchedByMonster)
        {
            // 선두 몬스터 속도로 플레이어를 밀기
            Rigidbody2D[] monsterRbs = monsterGroup.GetAllRigidbodies();
            if (monsterRbs.Length > 0)
                _rb.velocity = new Vector2(monsterRbs[0].velocity.x, _rb.velocity.y);
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
