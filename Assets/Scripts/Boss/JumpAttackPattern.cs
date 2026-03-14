using System;
using UnityEngine;

// X 이동 중지 → 점프(AddForce 1회) → 착지 시 근처 플레이어 넉백 → 이동 재개
// 발동 조건: 플레이어가 knockbackRadius 이내에 있을 때만 IsReady = true
// 점프 중에는 플레이어 콜라이더와 충돌을 무시하고, 착지 후 복구
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MonsterMover))]
public class JumpAttackPattern : BossPatternBase
{
    [Header("점프 설정")]
    [SerializeField] private float jumpForceY = 600f;
    [SerializeField] private string groundTag = "Ground";

    [Header("착지 넉백")]
    [SerializeField, Range(0f, 1f)] private float knockbackStrength = 1f;
    [SerializeField] private float knockbackRadius = 1.2f;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D _rb;
    private Collider2D _bossCollider;
    private Collider2D _playerCollider;
    private MonsterMover _mover;
    private Action _onComplete;
    private bool _isJumping;

    public override bool RequiresCloseRange => true;

    // 플레이어가 넉백 범위 안에 있을 때만 발동 가능
    public override bool IsReady
    {
        get
        {
            if (!base.IsReady) return false;
            if (_playerCollider == null) return false;
            float dist = Vector2.Distance(transform.position, _playerCollider.transform.position);
            return dist <= knockbackRadius;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bossCollider = GetComponent<Collider2D>();
        _mover = GetComponent<MonsterMover>();

        GameObject playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
            _playerCollider = playerGo.GetComponent<Collider2D>();
    }

    protected override void ExecutePattern(Transform owner, Transform target, Action onComplete)
    {
        _onComplete = onComplete;
        _isJumping = true;

        // 점프 중 플레이어 콜라이더와 충돌 무시
        if (_bossCollider != null && _playerCollider != null)
            Physics2D.IgnoreCollision(_bossCollider, _playerCollider, true);

        _mover.LockXMovement();
        _rb.AddForce(Vector2.up * jumpForceY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isJumping) return;
        if (!collision.gameObject.CompareTag(groundTag)) return;

        _isJumping = false;

        // 착지 후 플레이어 콜라이더 충돌 복구
        if (_bossCollider != null && _playerCollider != null)
            Physics2D.IgnoreCollision(_bossCollider, _playerCollider, false);

        // 착지 범위 내 플레이어 넉백
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, knockbackRadius, playerLayer);
        foreach (var col in hits)
            col.GetComponent<IKnockbackable>()?.ApplyKnockback(transform.position.x, knockbackStrength);

        _mover.UnlockXMovement();
        _onComplete?.Invoke();
        _onComplete = null;
    }

    protected override void OnResetState()
    {
        if (_isJumping && _bossCollider != null && _playerCollider != null)
            Physics2D.IgnoreCollision(_bossCollider, _playerCollider, false);

        _isJumping = false;
        _onComplete = null;
        _mover?.UnlockXMovement();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, knockbackRadius);
    }
#endif
}
