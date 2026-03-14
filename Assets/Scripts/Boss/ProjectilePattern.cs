using System;
using System.Collections;
using UnityEngine;

// 플레이어 방향으로 직선 투사체를 발사하는 패턴
public class ProjectilePattern : BossPatternBase
{
    [Header("투사체 설정")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField, Range(0f, 1f)] private float knockbackStrength = 0.5f;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float fireInterval = 0.2f;

    [Header("발사 위치")]
    [SerializeField] private Transform firePoint;  // null이면 owner 위치에서 발사

    public override bool RequiresCloseRange => false;

    protected override void ExecutePattern(Transform owner, Transform target, Action onComplete)
    {
        StartCoroutine(FireRoutine(owner, target, onComplete));
    }

    private IEnumerator FireRoutine(Transform owner, Transform target, Action onComplete)
    {
        Transform origin = firePoint != null ? firePoint : owner;
        Vector2 direction = (target.position - origin.position).normalized;
        // 보스는 항상 플레이어 오른쪽 — x 방향 강제 보정
        direction = new Vector2(-Mathf.Abs(direction.x), direction.y);

        for (int i = 0; i < projectileCount; i++)
        {
            SpawnProjectile(origin.position, direction);

            if (i < projectileCount - 1)
                yield return new WaitForSeconds(fireInterval);
        }

        yield return new WaitForSeconds(0.1f);
        onComplete();
    }

    private void SpawnProjectile(Vector3 origin, Vector2 direction)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("ProjectilePattern: projectilePrefab이 설정되지 않았습니다.");
            return;
        }

        GameObject go = Instantiate(projectilePrefab, origin, Quaternion.identity);
        BossProjectile proj = go.GetComponent<BossProjectile>();
        proj?.Launch(direction, projectileSpeed, knockbackStrength);
    }
}
