using System;
using System.Collections;
using UnityEngine;

// 보스가 왼쪽 방향으로 레이저를 발사하는 패턴
// 1단계: 얇은 레이저가 4회 깜빡임 (경고, 1초)
// 2단계: 굵은 레이저 발사 + 플레이어 넉백 (1회만)
[RequireComponent(typeof(LineRenderer))]
public class LaserPattern : BossPatternBase
{
    [Header("레이저 설정")]
    [SerializeField] private float laserLength = 30f;
    [SerializeField] private float laserDuration = 1.5f;
    [SerializeField, Range(0f, 1f)] private float knockbackStrength = 0.8f;

    [Header("경고 깜빡임 설정")]
    [SerializeField] private int blinkCount = 4;
    [SerializeField] private float blinkTotalDuration = 1f;
    [SerializeField] private float warningWidth = 0.05f;

    [Header("레이저 두께")]
    [SerializeField] private float activeWidth = 0.4f;

    [Header("레이저 색상")]
    [SerializeField] private Color warningColor = new Color(1f, 0.3f, 0.3f, 0.6f);
    [SerializeField] private Color activeColor  = new Color(1f, 0.1f, 0.1f, 1f);

    [Header("발사 위치")]
    [SerializeField] private Transform firePoint;

    [Header("충돌 판정")]
    [SerializeField] private LayerMask playerLayer;

    public override bool RequiresCloseRange => false;

    private LineRenderer _lr;
    private bool _playerHit;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 2;
        _lr.useWorldSpace = true;
        _lr.enabled = false;
    }

    protected override void ExecutePattern(Transform owner, Transform target, Action onComplete)
    {
        StartCoroutine(LaserRoutine(owner, target, onComplete));
    }

    private IEnumerator LaserRoutine(Transform owner, Transform target, Action onComplete)
    {
        _playerHit = false;

        Transform origin = firePoint != null ? firePoint : owner;

        // ─── 1단계: 경고 깜빡임 ────────────────────────────────────
        SetLaserStyle(warningWidth, warningColor);

        float blinkInterval = blinkTotalDuration / (blinkCount * 2f);
        for (int i = 0; i < blinkCount; i++)
        {
            UpdateLaserPositions(origin);
            _lr.enabled = true;
            yield return new WaitForSeconds(blinkInterval);

            _lr.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
        }

        // ─── 2단계: 실제 레이저 발사 ──────────────────────────────
        SetLaserStyle(activeWidth, activeColor);
        _lr.enabled = true;

        float elapsed = 0f;
        while (elapsed < laserDuration)
        {
            UpdateLaserPositions(origin);
            CheckPlayerHit(origin);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _lr.enabled = false;
        onComplete();
    }

    private void UpdateLaserPositions(Transform origin)
    {
        Vector3 start = origin.position;
        Vector3 end   = start + Vector3.left * laserLength;
        _lr.SetPosition(0, start);
        _lr.SetPosition(1, end);
    }

    // ─── 플레이어 충돌 판정 — 레이저 지속 시간 동안 1회만 적용 ──────
    private void CheckPlayerHit(Transform origin)
    {
        if (_playerHit) return;

        RaycastHit2D hit = Physics2D.Raycast(origin.position, Vector2.left, laserLength, playerLayer);
        if (hit.collider == null) return;

        IKnockbackable knockbackable = hit.collider.GetComponent<IKnockbackable>();
        if (knockbackable == null) return;

        _playerHit = true;
        knockbackable.ApplyKnockback(origin.position.x, knockbackStrength);
    }

    private void SetLaserStyle(float width, Color color)
    {
        _lr.startWidth = width;
        _lr.endWidth   = width;
        _lr.startColor = color;
        _lr.endColor   = color;
    }

    protected override void OnResetState()
    {
        _lr.enabled = false;
        _playerHit = false;
    }
}
