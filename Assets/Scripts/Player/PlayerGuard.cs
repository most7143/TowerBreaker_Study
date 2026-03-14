using System.Collections;
using UnityEngine;

public class PlayerGuard : MonoBehaviour, IGuardable
{
    [SerializeField] private MonsterGroup monsterGroup;
    [SerializeField] private PlayerUpgradeState upgradeState;

    private float _guardCooldown  = 2f;
    private float _guardPushForce = 12f;
    private float _guardPushDrag  = 4f;

    private PlayerAnimator _playerAnimator;
    private PlayerWallState _wallState;
    private bool _isGuarding;
    private bool _isGuardCooldown;

    public bool IsGuarding => _isGuarding;

    private void Awake()
    {
        PlayerStatsData data = Resources.Load<PlayerStatsData>("PlayerData/PlayerStatsData");
        if (data != null)
        {
            _guardCooldown  = data.guardCooldown;
            _guardPushForce = data.guardPushForce;
            _guardPushDrag  = data.guardPushDrag;
        }
        else
        {
            Debug.LogWarning("PlayerStatsData를 Resources/PlayerData 폴더에서 찾을 수 없습니다. 기본값을 사용합니다.");
        }

        _playerAnimator = GetComponent<PlayerAnimator>();
        _wallState = GetComponent<PlayerWallState>();
    }

    public void GuardStart()
    {
        if (_isGuardCooldown) return;
        if (_isGuarding) return;

        _isGuarding = true;
        _playerAnimator.PlayGuard();
        Debug.Log("Guard On");
    }

    public void GuardEnd()
    {
        if (!_isGuarding) return;

        _isGuarding = false;
        _playerAnimator.StopGuard();
        Debug.Log("Guard Off");
        StartCoroutine(GuardCooldownRoutine());
    }

    private IEnumerator GuardCooldownRoutine()
    {
        _isGuardCooldown = true;

        float cooldownReduction = upgradeState != null ? upgradeState.GuardCooldownReduction : 0f;
        float actualCooldown = Mathf.Max(0f, _guardCooldown - cooldownReduction);

        yield return new WaitForSeconds(actualCooldown);
        _isGuardCooldown = false;
        Debug.Log("Guard Cooldown End");
    }

    public void OnGuardHit()
    {
        if (!_isGuarding) return;

        monsterGroup.ResumeAllMonsters();

        float pushForceBonus = upgradeState != null ? upgradeState.GuardPushForceBonus : 0f;

        MonsterMover[] movers = monsterGroup.GetAllMovers();
        foreach (var mover in movers)
            mover.PushBack(_guardPushForce + pushForceBonus, _guardPushDrag);

        _wallState?.PushToWall();

        GuardEnd();

        Debug.Log("Guard Hit - pushed to wall");
    }
}
