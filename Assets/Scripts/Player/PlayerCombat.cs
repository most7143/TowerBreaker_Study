using UnityEngine;

// 애니메이션 이벤트를 수신해 공격 흐름을 연결하는 조율자
public class PlayerCombat : MonoBehaviour
{
    private PlayerGuard _playerGuard;
    private PlayerAnimator _playerAnimator;

    private void Awake()
    {
        _playerGuard = GetComponent<PlayerGuard>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    // 애니메이션 이벤트에서 호출 - 공격 한 타 종료
    public void OnAttackEnd()
    {
        // 공격 중에 가드를 눌렀다면, 공격 종료 후 가드 애니메이션 적용
        if (_playerGuard.IsGuarding)
            _playerAnimator.PlayGuard();
    }
}
