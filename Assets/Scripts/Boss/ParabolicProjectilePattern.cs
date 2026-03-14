using System;
using System.Collections;
using UnityEngine;

// 쿨타임마다 화면 내 랜덤 위치를 향해 포물선 투사체를 발사하는 패턴
// 포물선 물리: Vy = (dy - 0.5*g*t^2) / t,  Vx = dx / t
// t(비행 시간)은 Inspector에서 설정한 flightTime 고정값을 사용한다
public class ParabolicProjectilePattern : BossPatternBase
{
    [Header("투사체")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField, Range(0f, 1f)] private float knockbackStrength = 0.5f;

    [Header("발사 위치")]
    [SerializeField] private Transform firePoint;   // null이면 owner 위치 사용

    [Header("발사 설정")]
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float fireInterval = 0.3f;

    [Header("포물선 설정")]
    [SerializeField] private float flightTime = 1.2f;  // 목표 지점까지 비행 시간(초)

    [Header("랜덤 타겟 범위")]
    [SerializeField] private float rangeXMin = -8f;
    [SerializeField] private float rangeXMax =  8f;
    [SerializeField] private float rangeYMin = -4f;
    [SerializeField] private float rangeYMax =  0f;

    public override bool RequiresCloseRange => false;

    protected override void ExecutePattern(Transform owner, Transform target, Action onComplete)
    {
        Debug.Log("[ParabolicPattern] ExecutePattern 호출됨");
        StartCoroutine(FireRoutine(owner, onComplete));
    }

    private IEnumerator FireRoutine(Transform owner, Action onComplete)
    {
        Transform origin = firePoint != null ? firePoint : owner;
        Debug.Log($"[ParabolicPattern] 발사 origin: {origin.position}, firePoint 사용: {firePoint != null}");

        for (int i = 0; i < projectileCount; i++)
        {
            // 보스는 항상 플레이어 오른쪽 — 타겟 X를 origin보다 왼쪽으로 강제
            float originX = origin.position.x;
            float targetX = originX - Mathf.Abs(UnityEngine.Random.Range(rangeXMin, rangeXMax));
            Vector2 targetPos = new Vector2(targetX, UnityEngine.Random.Range(rangeYMin, rangeYMax));
            Debug.Log($"[ParabolicPattern] 랜덤 타겟 위치: {targetPos}");

            Vector2 initialVelocity = CalcParabolicVelocity(origin.position, targetPos, flightTime, gravityScale);
            Debug.Log($"[ParabolicPattern] 계산된 초속: {initialVelocity}, flightTime: {flightTime}, gravityScale: {gravityScale}");

            SpawnProjectile(origin.position, initialVelocity);

            if (i < projectileCount - 1)
                yield return new WaitForSeconds(fireInterval);
        }

        yield return new WaitForSeconds(0.1f);
        onComplete();
    }

    // 포물선 초속 계산
    // 물리식: pos(t) = pos0 + v0*t + 0.5*a*t^2
    //   → v0 = (target - origin - 0.5*a*t^2) / t
    // Unity Rigidbody2D의 중력 가속도 = Physics2D.gravity * gravityScale
    private Vector2 CalcParabolicVelocity(Vector2 origin, Vector2 target, float t, float gScale)
    {
        Vector2 gravity = Physics2D.gravity * gScale;
        Vector2 v0 = (target - origin - 0.5f * gravity * t * t) / t;
        return v0;
    }

    private void SpawnProjectile(Vector3 origin, Vector2 initialVelocity)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[ParabolicPattern] projectilePrefab이 null입니다. Inspector에서 연결하세요.");
            return;
        }

        GameObject go = Instantiate(projectilePrefab, origin, Quaternion.identity);
        Debug.Log($"[ParabolicPattern] 투사체 Instantiate 완료: {go.name} at {origin}");

        BossProjectile proj = go.GetComponent<BossProjectile>();
        if (proj == null)
        {
            Debug.LogError($"[ParabolicPattern] BossProjectile 컴포넌트를 찾을 수 없습니다. 프리팹: {go.name}");
            return;
        }

        proj.LaunchWithVelocity(initialVelocity, gravityScale, knockbackStrength);
        Debug.Log($"[ParabolicPattern] LaunchWithVelocity 호출 완료");
    }
}
