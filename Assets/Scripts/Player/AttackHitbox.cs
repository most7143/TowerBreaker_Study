using UnityEngine;

// 공격 히트박스 관리
// - 평소엔 Collider2D 비활성화
// - 애니메이션 이벤트 OnAttackHitboxOn / OnAttackHitboxOff 로 켜고 끔
// - IsTrigger = true 인 Collider2D 에 닿은 몬스터에게 데미지 적용
[RequireComponent(typeof(Collider2D))]
public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private int attackDamage = 20;

    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;
        _col.enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        MonsterStats monster = other.GetComponent<MonsterStats>();
        if (monster == null) return;

        monster.TakeDamage(attackDamage);
    }
}
