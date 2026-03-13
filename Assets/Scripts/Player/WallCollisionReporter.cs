using UnityEngine;

// 플레이어에 부착 - 벽 충돌 시 몬스터 전진 중지 + 플레이어 피해 처리
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class WallCollisionReporter : MonoBehaviour
{
    [SerializeField] private MonsterGroup monsterGroup;
    [SerializeField] private string wallTag = "Wall";

    private PlayerStats _playerStats;
    private PlayerWallState _wallState;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _wallState = GetComponent<PlayerWallState>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(wallTag)) return;

        _wallState.OnHitWall(transform.position.x);
        monsterGroup.OnPlayerHitWall();
    }
}
