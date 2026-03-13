using UnityEngine;

// 스테이지의 라운드 진행을 관리 - 몬스터가 전멸하면 다음 라운드로 진행
public class StageManager : MonoBehaviour
{
    [SerializeField] private StageData stageData;
    [SerializeField] private MonsterSpawner spawner;
    [SerializeField] private MonsterGroup monsterGroup;

    private int _currentRound = 0;

    private void Start()
    {
        StartRound(_currentRound);
    }

    private void StartRound(int roundIndex)
    {
        RoundData round = stageData.GetRound(roundIndex);
        monsterGroup.ResetState();
        spawner.SpawnRound(round);
        Debug.Log($"Round {roundIndex + 1} 시작 - 몬스터 {round.monsterCount}마리 / HP {round.monsterHp}");
    }

    // MonsterGroup이 몬스터 전멸을 감지하면 호출
    public void OnRoundCleared()
    {
        _currentRound++;

        if (_currentRound >= StageData.RoundCount)
        {
            Debug.Log("스테이지 클리어!");
            return;
        }

        StartRound(_currentRound);
    }
}
