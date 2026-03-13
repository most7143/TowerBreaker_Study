using System.Collections;
using UnityEngine;

// 플레이어가 벽에 눌린 상태를 관리
// - 벽 충돌 시 대시 불가
// - 공격/스킬/가드 발동 시 오른쪽으로 살짝 이동 후 대시 가능 복귀
public class PlayerWallState : MonoBehaviour
{
    [SerializeField] private float wallEscapeOffset = 1f;   // 공격/스킬/가드 발동 시 오른쪽 이동 거리
    [SerializeField] private float wallEscapeSpeed = 20f;   // 이동 속도

    private Rigidbody2D _rb;
    private MonsterContactHandler _contactHandler;
    private Coroutine _escapeRoutine;

    public bool IsPinnedToWall { get; private set; }
    public bool IsEscaping { get; private set; }

    private float _safeXPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _contactHandler = GetComponent<MonsterContactHandler>();
    }

    // 벽과 충돌해 피해를 입었을 때 호출
    public void OnHitWall(float wallContactX)
    {
        IsPinnedToWall = true;
        _safeXPosition = wallContactX + wallEscapeOffset;
        Debug.Log($"Wall pinned. SafeX = {_safeXPosition}");
    }

    // 공격/스킬/가드 발동 시 호출 - 오른쪽으로 이동하며 대시 가능 복귀
    public void EscapeFromWall()
    {
        if (!IsPinnedToWall) return;

        IsPinnedToWall = false;

        if (_escapeRoutine != null)
            StopCoroutine(_escapeRoutine);
        _escapeRoutine = StartCoroutine(EscapeRoutine());
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
