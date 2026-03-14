using System;
using UnityEngine;

[Serializable]
public class RoundData
{
    [Min(1)] public int monsterCount = 8;
    [Min(1)] public int monsterHp = 100;
    [Min(0)] public int coinPerMonster = 3;  // 몬스터 한 마리가 드롭하는 코인 수 (0이면 드롭 없음)
}

[Serializable]
public class BossRoundData
{
    [Min(1)] public int bossHp = 500;
    [Min(0)] public int coinOnDeath = 10;
}

// 스테이지 정보를 담는 ScriptableObject
[CreateAssetMenu(fileName = "StageData", menuName = "TowerBreaker/Stage Data")]
public class StageData : ScriptableObject
{
    [SerializeField] private GameObject monsterPrefab;
    public GameObject MonsterPrefab => monsterPrefab;

    // 설정하지 않으면 보스 라운드 없이 바로 클리어
    [SerializeField] private GameObject bossPrefab;
    public GameObject BossPrefab => bossPrefab;

    [SerializeField] private BossRoundData bossRound = new BossRoundData();
    public BossRoundData BossRound => bossRound;

    [SerializeField] private RoundData[] rounds = new RoundData[5];

    public int RoundCount => rounds.Length;

    public RoundData GetRound(int roundIndex)
    {
        if (roundIndex < 0 || roundIndex >= rounds.Length)
        {
            Debug.LogWarning($"RoundIndex {roundIndex} out of range.");
            return rounds[0];
        }
        return rounds[roundIndex];
    }

    private void OnValidate()
    {
        rounds ??= new RoundData[5];

        for (int i = 0; i < rounds.Length; i++)
            rounds[i] ??= new RoundData();
    }
}
