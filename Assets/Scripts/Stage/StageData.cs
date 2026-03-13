using System;
using UnityEngine;

[Serializable]
public class RoundData
{
    [Min(1)] public int monsterCount = 8;
    [Min(1)] public int monsterHp = 100;
    [Min(0)] public int coinPerMonster = 3;  // 몬스터 한 마리가 드롭하는 코인 수 (0이면 드롭 없음)
}

// 스테이지 정보를 담는 ScriptableObject - 5개 라운드 데이터 설정
[CreateAssetMenu(fileName = "StageData", menuName = "TowerBreaker/Stage Data")]
public class StageData : ScriptableObject
{
    public const int RoundCount = 5;

    [SerializeField] private RoundData[] rounds = new RoundData[RoundCount];

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
        if (rounds == null || rounds.Length != RoundCount)
            Array.Resize(ref rounds, RoundCount);

        for (int i = 0; i < rounds.Length; i++)
            rounds[i] ??= new RoundData();
    }
}
