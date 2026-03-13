using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, IAttackable
{
    public event Action OnAttackStarted;

    private PlayerAnimator _playerAnimator;
    private PlayerWallState _wallState;

    // 버튼을 누르고 있는 중인지
    private bool _holdingAttack;

    private void Awake()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
        _wallState = GetComponent<PlayerWallState>();
    }

    // 버튼 PointerDown
    public void AttackButtonDown()
    {
        _holdingAttack = true;
        TryAttack();
    }

    // 버튼 PointerUp
    public void AttackButtonUp()
    {
        _holdingAttack = false;
        _playerAnimator.SetAttack(false);
    }

    // IAttackable.Attack() - 외부(커맨드 등)에서 단발 호출용
    public void Attack()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        _wallState?.EscapeFromWall();
        OnAttackStarted?.Invoke();
        _playerAnimator.SetAttack(true);
        Debug.Log("Attack");
    }

    // AttackStateBehaviour의 OnStateExit에서 호출됨
    public void OnAttackStateExit()
    {
        if (_holdingAttack)
            TryAttack();
    }

    public void UseSkill(int skillIndex)
    {
        _wallState?.EscapeFromWall();
        _playerAnimator.PlaySkill(skillIndex);
        Debug.Log($"Skill {skillIndex}");
    }
}
