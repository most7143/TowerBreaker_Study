using UnityEngine;

// 공격 히트박스 관리
// - 평소엔 Collider2D 비활성화
// - 애니메이션 이벤트 OnAttackHitboxOn / OnAttackHitboxOff 로 켜고 끔
// - IsTrigger = true 인 Collider2D 에 닿은 몬스터에게 데미지 적용
[RequireComponent(typeof(Collider2D))]
public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private PlayerUpgradeState upgradeState;

    private int _attackDamage = 20;

    private Collider2D _col;

    private void Awake()
    {
        PlayerStatsData data = Resources.Load<PlayerStatsData>("PlayerData/PlayerStatsData");
        if (data != null)
            _attackDamage = data.attackPower;
        else
            Debug.LogWarning("PlayerStatsData를 Resources/PlayerData 폴더에서 찾을 수 없습니다. 기본값을 사용합니다.");

        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;
        _col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int bonus = upgradeState != null ? upgradeState.AttackPowerBonus : 0;
        int damage = _attackDamage + bonus;

        EnemyStats target = other.GetComponent<EnemyStats>();
        if (target == null) return;

        target.TakeDamage(damage);
    }
}
