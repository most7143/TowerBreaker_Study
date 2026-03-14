using System.Collections;
using UnityEngine;

// 개별 몬스터의 이동을 담당 - Rigidbody2D를 통한 물리 기반 이동
// MonsterGroup이 목표 위치를 제공하고, 이동은 MovePosition으로 처리
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MonsterMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool flipSprite = false;

    private Rigidbody2D _rb;
    private MonsterAnimator _monsterAnimator;
    private SpriteRenderer _spriteRenderer;
    private bool _isStopped;
    private bool _isPushedBack;
    private bool _isXMovementLocked;  // X 이동만 잠금 (물리/중력 유지)

    public bool IsStopped => _isStopped;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _monsterAnimator = GetComponent<MonsterAnimator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 중력으로 자연 착지, 회전만 고정
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // X 이동 잠금 — 물리/중력은 유지하고 MonsterGroup의 MoveTo만 차단
    public void LockXMovement()  { _isXMovementLocked = true; }
    public void UnlockXMovement() { _isXMovementLocked = false; }

    // MonsterGroup의 FixedUpdate에서 호출
    public void MoveTo(Vector3 targetPosition)
    {
        if (_isStopped || _isPushedBack || _isXMovementLocked)
        {
            if (!_isPushedBack)
                _rb.velocity = new Vector2(0f, _rb.velocity.y);
            _monsterAnimator.StopWalk();
            return;
        }

        float dist = targetPosition.x - transform.position.x;

        if (Mathf.Abs(dist) > 0.05f)
        {
            float direction = Mathf.Sign(dist);
            _rb.velocity = new Vector2(direction * moveSpeed, _rb.velocity.y);
            _spriteRenderer.flipX = flipSprite ? direction > 0f : direction < 0f;
            _monsterAnimator.PlayWalk();
        }
        else
        {
            _rb.velocity = new Vector2(0f, _rb.velocity.y);
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


    // 풀에서 꺼낼 때 초기 상태로 복구 - 중력으로 착지 후 Y 고정으로 전환
    public void ResetState()
    {
        _isStopped = false;
        _isPushedBack = false;
        _isXMovementLocked = false;
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
        _rb.velocity = new Vector2(velocityX, _rb.velocity.y);
        _spriteRenderer.flipX = flipSprite ? velocityX > 0f : velocityX < 0f;

        while (Mathf.Abs(_rb.velocity.x) > 0.05f)
        {
            _rb.velocity = new Vector2(
                Mathf.MoveTowards(_rb.velocity.x, 0f, drag * Time.fixedDeltaTime),
                _rb.velocity.y
            );
            yield return new WaitForFixedUpdate();
        }

        _rb.velocity = new Vector2(0f, _rb.velocity.y);
        _isPushedBack = false;
    }
}
