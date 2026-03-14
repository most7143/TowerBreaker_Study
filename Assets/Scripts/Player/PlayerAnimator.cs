using UnityEngine;

// 애니메이터 파라미터 이름 상수
public static class PlayerAnimParam
{
    public const string Attack      = "Attack";
    public const string AttackSpeed = "AttackSpeed";
    public const string Dash        = "Dash";
    public const string Guard       = "Guard";
    public const string Skill       = "Skill";
    public const string Hit         = "Hit";
}

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;

    private Animator Anim => _animator != null ? _animator : (_animator = GetComponent<Animator>());

    public void SetAttack(bool value) => Anim.SetBool(PlayerAnimParam.Attack, value);
    public void SetAttackSpeed(float speed) => Anim.SetFloat(PlayerAnimParam.AttackSpeed, speed);
    public void PlayDash()    => Anim.SetTrigger(PlayerAnimParam.Dash);
    public void PlayGuard()   => Anim.SetBool(PlayerAnimParam.Guard, true);
    public void StopGuard()   => Anim.SetBool(PlayerAnimParam.Guard, false);
    public void PlaySkill(int index) => Anim.SetTrigger(PlayerAnimParam.Skill + index);
    public void PlayHit()     => Anim.SetTrigger(PlayerAnimParam.Hit);

    public void ResetToIdle()
    {
        Anim.SetBool(PlayerAnimParam.Attack, false);
        Anim.SetBool(PlayerAnimParam.Guard, false);
        Anim.Play("Idle", 0, 0f);
    }
}
