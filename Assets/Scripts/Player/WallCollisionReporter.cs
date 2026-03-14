using System;
using UnityEngine;

// 플레이어에 부착 - 벽 충돌 시 이벤트 발행
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class WallCollisionReporter : MonoBehaviour
{
    [SerializeField] private string wallTag = "Wall";

    // 벽 충돌 시 발행 - MonsterGroup이 구독해 몬스터 전진 중지 처리
    public event Action OnHitWallEvent;

    private PlayerWallState _wallState;

    private void Awake()
    {
        _wallState = GetComponent<PlayerWallState>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(wallTag)) return;

        _wallState.OnHitWall(transform.position.x);
        OnHitWallEvent?.Invoke();
    }
}
