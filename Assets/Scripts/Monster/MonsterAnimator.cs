using UnityEngine;

public static class MonsterAnimParam
{
    public const string Walk = "Walk";
    public const string Hit = "Hit";
    public const string Die = "Death";
}

public class MonsterAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayWalk() => _animator.SetBool(MonsterAnimParam.Walk, true);
    public void StopWalk() => _animator.SetBool(MonsterAnimParam.Walk, false);
    public void PlayHit() => _animator.SetTrigger(MonsterAnimParam.Hit);
    public void PlayDie() => _animator.SetTrigger(MonsterAnimParam.Die);
}
