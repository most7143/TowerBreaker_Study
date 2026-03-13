using UnityEngine;

// Animator의 Attack 상태에 붙이는 StateMachineBehaviour
// 애니메이션 이벤트 없이 상태 진입/이탈을 감지한다
public class AttackStateBehaviour : StateMachineBehaviour
{
    private PlayerAttack _playerAttack;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerAttack == null)
            _playerAttack = animator.GetComponent<PlayerAttack>();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Attack 상태에서 벗어날 때 (다른 동작으로 전환돼도 정확히 한 번만 호출)
        _playerAttack.OnAttackStateExit();
    }
}
