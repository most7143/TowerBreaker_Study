public class MonsterStats : EnemyStats
{
    protected override void OnDead()
    {
        MonsterGroup?.RemoveMonster(MonsterMover);
        MonsterCountUI.Instance.HideOne();
        DropCoin();
        MonsterMover.Stop();
        GetComponent<UnityEngine.Collider2D>().enabled = false;
        MonsterAnimator.PlayDie();
    }

    public override void OnDeathAnimationEnd()
    {
        MonsterPool.Instance.Return(gameObject);
    }

    // 풀에서 꺼낼 때 MonsterSpawner가 호출
    public void ResetState(MonsterGroup group, int hp, int coinDrop)
    {
        Initialize(group, hp, coinDrop);
    }

}
