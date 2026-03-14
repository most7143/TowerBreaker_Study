using UnityEngine;

// 보스가 발사하는 투사체
// 직선: Launch(direction, speed, ...)
// 포물선: LaunchWithVelocity(initialVelocity, gravityScale, ...)
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BossProjectile : MonoBehaviour
{
    private float _knockbackStrength;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.isKinematic = false;
    }

    // 직선 발사
    public void Launch(Vector2 direction, float speed, float knockbackStrength)
    {
        _knockbackStrength = knockbackStrength;
        _rb.gravityScale = 0f;
        _rb.velocity = direction.normalized * speed;
    }

    // 포물선 발사 — 초속과 중력 스케일을 패턴에서 계산해서 전달
    public void LaunchWithVelocity(Vector2 initialVelocity, float gravityScale, float knockbackStrength)
    {
        _knockbackStrength = knockbackStrength;
        _rb.gravityScale = gravityScale;
        _rb.velocity = initialVelocity;
        Debug.Log($"[BossProjectile] LaunchWithVelocity — velocity: {initialVelocity}, gravityScale: {gravityScale}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[BossProjectile] OnTriggerEnter2D — 충돌 대상: {other.gameObject.name}, tag: {other.tag}, isTrigger: {other.isTrigger}");

        IKnockbackable knockbackable = other.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            Debug.Log($"[BossProjectile] 플레이어 넉백 적용 후 소멸");
            knockbackable.ApplyKnockback(transform.position.x, _knockbackStrength);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger && !other.CompareTag("Monster"))
        {
            Debug.Log($"[BossProjectile] 고체 충돌체에 닿아 소멸: {other.gameObject.name}");
            Destroy(gameObject);
        }
    }
}
