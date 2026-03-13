using UnityEngine;
using UnityEngine.Events;

public class MonsterStats : CharacterBase
{
    public UnityAction<Vector3, int> OnDiedWithCoin;

    private MonsterAnimator _monsterAnimator;
    private MonsterMover _monsterMover;
    private MonsterGroup _monsterGroup;
    private int _coinDrop;

    protected override void Awake()
    {
        base.Awake();
        _monsterAnimator = GetComponent<MonsterAnimator>();
        _monsterMover = GetComponent<MonsterMover>();
    }

    public void RegisterGroup(MonsterGroup group)
    {
        _monsterGroup = group;
    }

    protected override void OnDamaged(int amount)
    {
        _monsterAnimator.PlayHit();
        Debug.Log($"Monster Hit: {currentHp}/{maxHp}");
    }

    protected override void OnDead()
    {
        // 대열에서 먼저 제거해 이동 연산에서 빠짐
        _monsterGroup?.RemoveMonster(_monsterMover);

        // UI 아이콘 하나 숨김
        MonsterCountUI.Instance.HideOne();

        // 코인 드롭 이벤트 발행
        if (_coinDrop > 0)
            OnDiedWithCoin?.Invoke(transform.position, _coinDrop);

        // 이동·충돌 중지
        _monsterMover.Stop();
        GetComponent<Collider2D>().enabled = false;

        // 사망 애니메이션 재생 - 끝나면 Animation Event OnDeathAnimationEnd() 호출
        _monsterAnimator.PlayDie();
    }

    // 사망 애니메이션 마지막 프레임의 Animation Event에서 호출
    public void OnDeathAnimationEnd()
    {
        MonsterPool.Instance.Return(gameObject);
    }

    // 풀에서 꺼낼 때 MonsterSpawner가 호출 - 모든 상태 초기화
    public void ResetState(MonsterGroup group, int hp, int coinDrop)
    {
        maxHp = hp;
        currentHp = maxHp;
        _coinDrop = coinDrop;
        _monsterGroup = group;
        GetComponent<Collider2D>().enabled = true;
        _monsterMover.ResetState();
    }
}
