using UnityEngine;
using UnityEngine.Events;

// 몬스터·보스가 공유하는 적 스텟 기반 클래스
// 외부(AttackHitbox 등)에서는 이 타입으로만 참조한다
public abstract class EnemyStats : CharacterBase
{
    public UnityAction<Vector3, int> OnDiedWithCoin;

    protected MonsterAnimator MonsterAnimator { get; private set; }
    protected MonsterMover    MonsterMover    { get; private set; }
    protected MonsterGroup    MonsterGroup    { get; private set; }
    protected int             CoinDrop        { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        MonsterAnimator = GetComponent<MonsterAnimator>();
        MonsterMover    = GetComponent<MonsterMover>();
    }

    // 스폰 시 공통 초기화 — MonsterSpawner / MonsterGroup에서 호출
    public virtual void Initialize(MonsterGroup group, int hp, int coinDrop)
    {
        maxHp      = hp;
        currentHp  = hp;
        CoinDrop   = coinDrop;
        MonsterGroup = group;

        GetComponent<Collider2D>().enabled = true;
        MonsterMover?.ResetState();
    }

    protected override void OnDamaged(int amount)
    {
        MonsterAnimator?.PlayHit();
        Debug.Log($"[{GetType().Name}] Hit: {currentHp}/{maxHp}");
    }

    // 코인 드롭 — 하위 클래스 OnDead에서 호출
    protected void DropCoin()
    {
        if (CoinDrop > 0)
            OnDiedWithCoin?.Invoke(transform.position, CoinDrop);
    }

    // 사망 처리 (풀 반환 / Destroy 등)는 하위 클래스마다 다름
    protected abstract override void OnDead();

    // 사망 애니메이션 마지막 프레임 Animation Event — 하위 클래스에서 구현
    public abstract void OnDeathAnimationEnd();
}
