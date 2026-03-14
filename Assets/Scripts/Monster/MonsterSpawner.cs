using UnityEngine;

// 라운드 데이터에 따라 몬스터를 스폰
public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private MonsterGroup monsterGroup;
    [SerializeField] private CoinDropper coinDropper;

    [SerializeField] private Vector3 spawnOrigin = new Vector3(8f, 0f, 0f);   // 첫 번째(선두) 몬스터 위치
    [SerializeField] private float spacing = 1f;   // MonsterGroup의 monsterSpacing과 일치시킬 것

    public void SpawnRound(RoundData round)
    {
        MonsterCountUI.Instance.Initialize(round.monsterCount);

        for (int i = 0; i < round.monsterCount; i++)
        {
            Vector3 pos = spawnOrigin + new Vector3(i * spacing, 0f, 0f);
            GameObject go = MonsterPool.Instance.Rent(pos);

            MonsterStats stats = go.GetComponent<MonsterStats>();
            if (stats != null)
            {
                stats.ResetState(monsterGroup, round.monsterHp, round.coinPerMonster);
                if (coinDropper != null)
                    stats.OnDiedWithCoin += coinDropper.Drop;
            }

            MonsterMover mover = go.GetComponent<MonsterMover>();
            if (mover != null)
                monsterGroup.AddMonster(mover);
        }
    }

    // 보스 1마리를 스폰 — 풀 없이 직접 Instantiate (라운드당 1회)
    public void SpawnBoss(GameObject bossPrefab, BossRoundData bossRound)
    {
        MonsterCountUI.Instance.Initialize(1);

        GameObject go = Instantiate(bossPrefab, spawnOrigin, Quaternion.identity);

        BossStats bossStats = go.GetComponent<BossStats>();
        if (bossStats != null)
        {
            bossStats.Initialize(monsterGroup, bossRound.bossHp, bossRound.coinOnDeath);
            if (coinDropper != null)
                bossStats.OnDiedWithCoin += coinDropper.Drop;
        }

        MonsterMover mover = go.GetComponent<MonsterMover>();
        if (mover != null)
            monsterGroup.AddMonster(mover);

        // 플레이어의 MonsterContactHandler에 보스 BossAIBrain 동적 등록
        BossAIBrain brain = go.GetComponent<BossAIBrain>();
        GameObject playerGo = GameObject.FindWithTag("Player");
        if (brain != null && playerGo != null)
        {
            MonsterContactHandler contactHandler = playerGo.GetComponent<MonsterContactHandler>();
            contactHandler?.SetBoss(brain);
        }
    }
}
