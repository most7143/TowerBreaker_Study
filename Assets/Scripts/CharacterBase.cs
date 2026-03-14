using UnityEngine;

// 플레이어와 몬스터가 공유하는 HP, 공격력 공통 기반 클래스
public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected int maxHp = 100;
    [SerializeField] protected int attackPower = 10;
    protected int currentHp;

    private Rigidbody2D _rb;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public int AttackPower => attackPower;
    public bool IsDead => currentHp <= 0;

    protected virtual void Awake()
    {
        currentHp = maxHp;
        _rb = GetComponent<Rigidbody2D>();
    }

    // x 위치만 변경하고 velocity를 초기화해 중력으로 바닥에 자연 착지
    public void ResetPositionX(float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        if (_rb != null)
            _rb.velocity = Vector2.zero;
    }

    public virtual void TakeDamage(int amount)
    {
        if (IsDead) return;
        currentHp = Mathf.Max(0, currentHp - amount);
        OnDamaged(amount);
        if (IsDead) OnDead();
    }

    // 피격 시 처리 (하위 클래스에서 오버라이드 가능)
    protected virtual void OnDamaged(int amount) { }

    // 사망 시 처리 (하위 클래스에서 오버라이드 가능)
    protected virtual void OnDead() { }
}
