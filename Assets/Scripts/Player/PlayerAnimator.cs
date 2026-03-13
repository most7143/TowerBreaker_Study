using UnityEngine;

// 애니메이터 파라미터 이름 상수
public static class PlayerAnimParam
{
    public const string Attack = "Attack";
    public const string Dash   = "Dash";
    public const string Guard  = "Guard";
    public const string Skill  = "Skill";
    public const string Hit    = "Hit";
}

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAttack(bool value) => _animator.SetBool(PlayerAnimParam.Attack, value);
    public void PlayDash()    => _animator.SetTrigger(PlayerAnimParam.Dash);
    public void PlayGuard()   => _animator.SetBool(PlayerAnimParam.Guard, true);
    public void StopGuard()   => _animator.SetBool(PlayerAnimParam.Guard, false);
    public void PlaySkill(int index) => _animator.SetTrigger(PlayerAnimParam.Skill + index);
    public void PlayHit()     => _animator.SetTrigger(PlayerAnimParam.Hit);
}
