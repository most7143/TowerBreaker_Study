using System;
using UnityEngine;

// 보스 패턴 공통 베이스
// 쿨타임 추적과 실행 중 여부를 여기서 관리
// 구체 패턴은 ExecutePattern() 만 구현하면 된다
public abstract class BossPatternBase : MonoBehaviour, IBossPattern
{
    [Header("공통 패턴 설정")]
    [SerializeField] private float cooldown = 5f;
    [SerializeField] private bool stopsMovement = false;  // 패턴 실행 중 보스 이동 중단 여부

    private float _lastUsedTime = -999f;
    private bool _isRunning;

    // ─── IBossPattern 구현 ───────────────────────────────────────
    public virtual bool IsReady => !_isRunning && Time.time >= _lastUsedTime + cooldown;
    public bool StopsMovement => stopsMovement;

    // 구체 클래스에서 재정의
    public abstract bool RequiresCloseRange { get; }

    public void Execute(Transform owner, Transform target, Action onComplete)
    {
        if (_isRunning) return;
        _isRunning = true;
        _lastUsedTime = Time.time;
        ExecutePattern(owner, target, () =>
        {
            _isRunning = false;
            onComplete?.Invoke();
        });
    }

    public void ResetState()
    {
        _lastUsedTime = -999f;
        _isRunning = false;
        StopAllCoroutines();
        OnResetState();
    }

    // ─── 구체 클래스가 구현할 메서드 ─────────────────────────────
    // 실제 패턴 로직을 담는다 — 완료 시 반드시 onComplete() 호출
    protected abstract void ExecutePattern(Transform owner, Transform target, Action onComplete);

    // 추가 리셋 처리가 필요한 경우 구체 클래스에서 override
    protected virtual void OnResetState() { }
}
