using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour, IDashable
{
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float stopDistanceFromMonster = 1f;
    [SerializeField] private float minDashDistance = 1.5f;  // 몬스터가 이 거리 이내면 대시 불가
    [SerializeField] private MonsterGroup monsterGroup;
    [SerializeField] private Transform dashLimitPoint;  // 이 X 위치까지만 대시 가능

    private PlayerAnimator _playerAnimator;
    private Rigidbody2D _rb;
    private bool _isDashing;

    private void Awake()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Dash()
    {
        if (_isDashing) return;

        MonsterMover[] movers = monsterGroup.GetAllMovers();
        if (movers.Length == 0) return;

        MonsterMover target = null;
        for (int i = 0; i < movers.Length; i++)
        {
            if (movers[i] != null && movers[i].gameObject.activeInHierarchy)
            {
                target = movers[i];
                break;
            }
        }
        if (target == null) return;

        // 가장 가까운 몬스터가 너무 가까우면 대시 불가
        float distToMonster = target.transform.position.x - transform.position.x;
        if (distToMonster <= minDashDistance) return;

        // 목적지는 x축만 이동, y는 플레이어 현재 y 유지
        float destinationX = target.transform.position.x - stopDistanceFromMonster;

        // 대시 제한 지점이 설정된 경우, 그 X를 넘지 않도록 클램프
        if (dashLimitPoint != null)
            destinationX = Mathf.Min(destinationX, dashLimitPoint.position.x);

        Vector3 destination = new Vector3(destinationX, transform.position.y, transform.position.z);

        // 목적지 기준으로 거리 확인
        if (transform.position.x >= destinationX) return;

        monsterGroup.ResumeAllMonsters();
        _playerAnimator.PlayDash();
        StartCoroutine(DashCoroutine(destination));
    }

    private IEnumerator DashCoroutine(Vector3 destination)
    {
        _isDashing = true;

        // 대시 중 물리 충돌로 몬스터가 밀리지 않도록 kinematic 전환
        RigidbodyType2D originalType = _rb.bodyType;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.velocity = Vector2.zero;

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashDuration;
            transform.position = new Vector3(
                Mathf.Lerp(startPos.x, destination.x, t),
                transform.position.y,
                transform.position.z
            );
            yield return null;
        }

        transform.position = destination;

        _rb.bodyType = originalType;
        _isDashing = false;
    }
}
