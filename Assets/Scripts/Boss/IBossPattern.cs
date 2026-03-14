// 보스 패턴의 공통 계약
// BossAIBrain은 이 인터페이스만 바라보며 구체 패턴 클래스를 모른다
using UnityEngine;

public interface IBossPattern
{
    // 패턴이 발동 가능한 상태인지 (쿨타임 완료 여부)
    bool IsReady { get; }

    // 근접 우선 패턴인지 — 쿨타임이 동일할 때 플레이어가 근접 범위 내에 있으면 이 패턴 우선
    bool RequiresCloseRange { get; }

    // 패턴 실행 중 MonsterMover 이동을 멈출지 여부
    bool StopsMovement { get; }

    // 패턴 실행 — BossAIBrain이 Casting 상태 진입 시 1회 호출
    // 실행이 끝나면 onComplete 콜백을 호출해 Brain에게 알린다
    void Execute(Transform owner, Transform target, System.Action onComplete);

    // 패턴 초기화 (보스 리스폰 등 상태 리셋 시)
    void ResetState();
}
