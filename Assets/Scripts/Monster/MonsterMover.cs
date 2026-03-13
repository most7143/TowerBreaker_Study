using System.Collections;
using UnityEngine;

// 개별 몬스터의 이동을 담당 - Rigidbody2D를 통한 물리 기반 이동
// MonsterGroup이 목표 위치를 제공하고, 이동은 MovePosition으로 처리
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MonsterMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D _rb;
    private MonsterAnimator _monsterAnimator;
    private bool _isStopped;
    private bool _isPushedBack;

    public bool IsStopped => _isStopped;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _monsterAnimator = GetComponent<MonsterAnimator>();

        // 2D 횡스크롤 게임: 중력 없음, 회전 고정
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
    }

    // MonsterGroup의 FixedUpdate에서 호출
    public void MoveTo(Vector3 targetPosition)
    {
        if (_isStopped || _isPushedBack)
        {
            if (!_isPushedBack)
                _rb.velocity = Vector2.zero;
            _monsterAnimator.StopWalk();
            return;
        }

        float dist = targetPosition.x - transform.position.x;

        if (Mathf.Abs(dist) > 0.05f)
        {
            float direction = Mathf.Sign(dist);
            _rb.velocity = new Vector2(direction * moveSpeed, 0f);
            _monsterAnimator.PlayWalk();
        }
        else
        {
            _rb.velocity = Vector2.zero;
            _monsterAnimator.StopWalk();
        }
    }

    public void Stop()
    {
        _isStopped = true;
        _rb.velocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _monsterAnimator.StopWalk();
    }

    public void Resume()
    {
        if (!_isStopped) return;
        _isStopped = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // 풀에서 꺼낼 때 초기 상태로 복구
    public void ResetState()
    {
        _isStopped = false;
        _isPushedBack = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.velocity = Vector2.zero;
        StopAllCoroutines();
    }

    // 가드 반동으로 밀려남 - 자연스럽게 감속 후 복귀
    public void PushBack(float velocityX, float drag = 4f)
    {
        StartCoroutine(PushBackRoutine(velocityX, drag));
    }

    private IEnumerator PushBackRoutine(float velocityX, float drag)
    {
        _isPushedBack = true;
        _rb.velocity = new Vector2(velocityX, 0f);

        while (Mathf.Abs(_rb.velocity.x) > 0.05f)
        {
            _rb.velocity = new Vector2(
                Mathf.MoveTowards(_rb.velocity.x, 0f, drag * Time.fixedDeltaTime),
                0f
            );
            yield return new WaitForFixedUpdate();
        }

        _rb.velocity = Vector2.zero;
        _isPushedBack = false;
    }
}
