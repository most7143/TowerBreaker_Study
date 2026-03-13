using System.Collections;
using UnityEngine;

public class PlayerGuard : MonoBehaviour, IGuardable
{
    [SerializeField] private float guardCooldown = 2f;
    [SerializeField] private float guardPushForce = 12f;
    [SerializeField] private float guardPushDrag = 4f;
    [SerializeField] private MonsterGroup monsterGroup;

    private PlayerAnimator _playerAnimator;
    private PlayerWallState _wallState;
    private bool _isGuarding;
    private bool _isGuardCooldown;

    public bool IsGuarding => _isGuarding;

    private void Awake()
    {
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
        yield return new WaitForSeconds(guardCooldown);
        _isGuardCooldown = false;
        Debug.Log("Guard Cooldown End");
    }

    public void OnGuardHit()
    {
        if (!_isGuarding) return;

        bool wasPinnedToWall = _wallState != null && _wallState.IsPinnedToWall;
        float pushForce = wasPinnedToWall ? guardPushForce * 0.2f : guardPushForce;

        monsterGroup.ResumeAllMonsters();

        MonsterMover[] movers = monsterGroup.GetAllMovers();
        foreach (var mover in movers)
            mover.PushBack(pushForce, guardPushDrag);

        _wallState?.EscapeFromWall();

        GuardEnd();

        Debug.Log($"Guard Hit - push force: {pushForce} (wall escape: {wasPinnedToWall})");
    }
}
