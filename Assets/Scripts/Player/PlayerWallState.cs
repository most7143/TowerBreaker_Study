using System.Collections;
using UnityEngine;

// 플레이어가 벽에 눌린 상태를 관리
// - 벽 충돌 시 대시 불가
// - 공격/스킬/가드 발동 시 오른쪽으로 살짝 이동 후 대시 가능 복귀
// - 가드 성공 시 벽쪽으로 강제 밀려남 (PushToWall)
public class PlayerWallState : MonoBehaviour
{
    [SerializeField] private float wallEscapeOffset = 1f;   // 공격/스킬/가드 발동 시 오른쪽 이동 거리
    [SerializeField] private float wallEscapeSpeed = 20f;   // 이동 속도
    [SerializeField] private float guardPushSpeed = 30f;    // 가드 성공 시 벽쪽으로 밀리는 속도
    [SerializeField] private Transform wallTransform;       // 씬의 벽 오브젝트 (PushToWall에서 위치 참조)

    private Rigidbody2D _rb;
    private MonsterContactHandler _contactHandler;
    private Coroutine _escapeRoutine;
    private Coroutine _pushToWallRoutine;
    private Coroutine _knockbackRoutine;

    public bool IsPinnedToWall { get; private set; }
    public bool IsEscaping { get; private set; }
    public bool IsBeingPushed { get; private set; }
    public Transform WallTransform => wallTransform;

    private float _safeXPosition;
    private float _wallX;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _contactHandler = GetComponent<MonsterContactHandler>();
    }

    // 벽과 충돌해 피해를 입었을 때 호출
    public void OnHitWall(float wallContactX)
    {
        IsPinnedToWall = true;
        _wallX = wallTransform != null ? wallTransform.position.x : wallContactX;
        _safeXPosition = _wallX + wallEscapeOffset;
        Debug.Log($"Wall pinned. SafeX = {_safeXPosition}");
    }

    // 공격/스킬/가드 발동 시 호출 - 오른쪽으로 이동하며 대시 가능 복귀
    public void EscapeFromWall()
    {
        if (!IsPinnedToWall) return;

        // 이미 safeX 오른쪽에 있으면 탈출 완료로 처리하고 이동 스킵
        if (_rb.position.x > _safeXPosition)
        {
            IsPinnedToWall = false;
            _contactHandler?.ClearMonsterContact();
            return;
        }

        IsPinnedToWall = false;

        if (_escapeRoutine != null)
            StopCoroutine(_escapeRoutine);
        _escapeRoutine = StartCoroutine(EscapeRoutine());
    }

    // 가드 성공 시 호출 - 벽의 safeX 위치까지 빠르게 밀려남, 조작 불가
    public void PushToWall()
    {
        // 벽 위치: wallTransform 우선, 없으면 마지막으로 기록된 _wallX 사용
        if (wallTransform != null)
            _wallX = wallTransform.position.x;
        else if (!IsPinnedToWall)
            return; // 벽 위치를 알 수 없으면 스킵

        IsPinnedToWall = false;

        if (_escapeRoutine != null)
        {
            StopCoroutine(_escapeRoutine);
            _escapeRoutine = null;
        }

        if (_pushToWallRoutine != null)
            StopCoroutine(_pushToWallRoutine);
        _pushToWallRoutine = StartCoroutine(PushToWallRoutine());
    }

    private IEnumerator PushToWallRoutine()
    {
        IsBeingPushed = true;

        // 벽에서 wallEscapeOffset만큼 앞이 목적지
        float targetX = _wallX + wallEscapeOffset;

        while (Mathf.Abs(_rb.position.x - targetX) > 0.02f)
        {
            float newX = Mathf.MoveTowards(_rb.position.x, targetX, guardPushSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(new Vector2(newX, _rb.position.y));
            yield return new WaitForFixedUpdate();
        }

        _rb.MovePosition(new Vector2(targetX, _rb.position.y));
        _rb.velocity = new Vector2(0f, _rb.velocity.y);

        IsPinnedToWall = true;
        _safeXPosition = targetX;
        _contactHandler?.ClearMonsterContact();

        IsBeingPushed = false;
        _pushToWallRoutine = null;
    }

    // 보스 공격에 의한 넉백 — 공격 발생 위치의 반대 방향으로 밀어냄
    public void ApplyBossKnockback(float force)
    {
        if (_knockbackRoutine != null)
            StopCoroutine(_knockbackRoutine);
        IsBeingPushed = true;  // 코루틴 시작 전 즉시 설정해 같은 프레임 MonsterContactHandler 덮어쓰기 방지
        _knockbackRoutine = StartCoroutine(KnockbackRoutine(force));
    }

    private IEnumerator KnockbackRoutine(float force)
    {
        IsBeingPushed = true;

        // 이전 넉백 velocity를 초기화한 뒤 새 force 적용
        _rb.velocity = new Vector2(0f, _rb.velocity.y);
        _rb.velocity = new Vector2(-force, _rb.velocity.y);

        // 속도가 줄어들 때까지 대기 (drag는 Rigidbody2D의 Linear Drag에 위임)
        yield return new WaitUntil(() => Mathf.Abs(_rb.velocity.x) < 0.5f);

        _rb.velocity = new Vector2(0f, _rb.velocity.y);
        IsBeingPushed = false;
        _knockbackRoutine = null;
    }

    private IEnumerator EscapeRoutine()
    {
        IsEscaping = true;

        float targetX = _safeXPosition;

        while (Mathf.Abs(_rb.position.x - targetX) > 0.02f)
        {
            float newX = Mathf.MoveTowards(_rb.position.x, targetX, wallEscapeSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(new Vector2(newX, _rb.position.y));
            yield return new WaitForFixedUpdate();
        }

        _rb.MovePosition(new Vector2(targetX, _rb.position.y));
        _rb.velocity = new Vector2(0f, _rb.velocity.y);

        // 탈출 완료 - 몬스터 접촉 상태 초기화
        _contactHandler?.ClearMonsterContact();

        IsEscaping = false;
        _escapeRoutine = null;
    }
}
