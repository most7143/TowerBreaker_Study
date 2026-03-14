using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : CharacterBase, IKnockbackable
{
    [SerializeField] private PlayerUpgradeState upgradeState;

    public event Action OnPlayerDead;
    public event Action<int, int> OnHpChanged;  // (currentHp, maxHp)

    private PlayerAnimator _playerAnimator;
    private PlayerWallState _wallState;
    private PlayerController _playerController;
    private PlayerGuard _playerGuard;
    private bool _isInvincible;

    private PlayerStatsData _data;

    protected override void Awake()
    {
        _data = Resources.Load<PlayerStatsData>("PlayerData/PlayerStatsData");
        if (_data != null)
        {
            maxHp       = _data.maxHp;
            attackPower = _data.attackPower;
        }
        else
        {
            Debug.LogWarning("PlayerStatsData를 Resources/PlayerData 폴더에서 찾을 수 없습니다. 기본값을 사용합니다.");
        }

        base.Awake();

        _playerAnimator   = GetComponent<PlayerAnimator>();
        _wallState        = GetComponent<PlayerWallState>();
        _playerController = GetComponent<PlayerController>();
        _playerGuard      = GetComponent<PlayerGuard>();
    }

    private void Start()
    {
        ApplyUpgrades();
    }

    // 업그레이드 팝업 닫을 때 호출 — 강화 수치를 애니메이터/스탯에 반영
    public void ApplyUpgrades()
    {
        if (_data == null) return;

        float baseSpeed = _data.attackSpeed;
        float speedBonus = upgradeState != null ? upgradeState.AttackSpeedBonus : 0f;
        _playerAnimator?.SetAttackSpeed(baseSpeed + speedBonus);
    }

    // IKnockbackable — 보스 패턴이 PlayerStats를 직접 참조하지 않고 이 인터페이스를 통해 호출
    public void ApplyKnockback(float knockbackSourceX, float knockbackStrength)
    {
        // 가드 중이면 피해/넉백 없이 가드 소비 처리
        if (_playerGuard != null && _playerGuard.IsGuarding)
        {
            _wallState?.PushToWall();
            _playerGuard.GuardEnd();
            return;
        }

        float force = KnockbackValues.Get(knockbackStrength);
        _wallState?.ApplyBossKnockback(force);
    }

    public override void TakeDamage(int amount)
    {
        if (_isInvincible) return;
        base.TakeDamage(amount);
    }

    // 벽 충돌 피격 — 히트 애니메이션 + 무적 + 입력 잠금
    public void OnWallHit(int damage, float invincibleDuration = 0.5f)
    {
        if (_isInvincible) return;
        _playerAnimator?.PlayHit();
        base.TakeDamage(damage);
        if (!IsDead)
            StartCoroutine(InvincibleRoutine(invincibleDuration));
    }

    private IEnumerator InvincibleRoutine(float duration)
    {
        _isInvincible = true;
        _playerController?.LockInput();

        yield return new WaitForSeconds(duration);

        _isInvincible = false;
        _playerController?.UnlockInput();
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Min(maxHp, currentHp + amount);
        OnHpChanged?.Invoke(currentHp, maxHp);
        Debug.Log($"Player Heal +{amount} → HP: {currentHp}/{maxHp}");
    }

    protected override void OnDamaged(int amount)
    {
        Debug.Log($"Player HP: {currentHp}/{maxHp}");
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    protected override void OnDead()
    {
        Debug.Log("게임 오버");
        OnHpChanged?.Invoke(0, maxHp);
        OnPlayerDead?.Invoke();
    }
}
