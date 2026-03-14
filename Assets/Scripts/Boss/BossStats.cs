using UnityEngine;

// 보스 전용 Stats — 풀링 없이 Instantiate/Destroy로 관리
// MonsterGroup에 사망을 통보해 OnRoundCleared 이벤트 발행
public class BossStats : EnemyStats
{
    private BossAIBrain _brain;

    protected override void Awake()
    {
        base.Awake();
        _brain = GetComponent<BossAIBrain>();
    }

    // MonsterSpawner.SpawnBoss()에서 호출 — 플레이어는 BossAIBrain이 태그로 자동 탐색
    public override void Initialize(MonsterGroup group, int hp, int coinDrop)
    {
        base.Initialize(group, hp, coinDrop);
        _brain?.Initialize(group);
    }

    protected override void OnDead()
    {
        _brain?.OnBossDead();

        // 플레이어 MonsterContactHandler의 보스 참조 해제
        GameObject playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
            playerGo.GetComponent<MonsterContactHandler>()?.SetBoss(null);

        MonsterGroup?.RemoveMonster(MonsterMover);
        MonsterCountUI.Instance?.HideOne();
        DropCoin();

        MonsterMover?.Stop();
        GetComponent<Collider2D>().enabled = false;

        MonsterAnimator?.PlayDie();
        // 사망 애니메이션 끝에 Animation Event로 OnDeathAnimationEnd() 호출
    }

    public override void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}
