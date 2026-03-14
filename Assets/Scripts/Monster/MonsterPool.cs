using UnityEngine;

// 몬스터 오브젝트 풀 — ObjectPool<MonsterMover>를 상속
public class MonsterPool : ObjectPool<MonsterMover>
{
    public static MonsterPool Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        // prefab이 Inspector에 설정된 경우에만 Prewarm (StageData에서 설정하는 경우 Start에서 처리)
        if (prefab != null)
            base.Awake();
    }

    // 스테이지 시작 시 StageData의 monsterPrefab으로 풀 초기화
    public void InitializeWithPrefab(GameObject monsterPrefab)
    {
        prefab = monsterPrefab;
        RebuildPool();
    }

    // GameObject 기반 API를 유지해 MonsterSpawner 호출부 변경 최소화
    public GameObject Rent(Vector3 position)
    {
        return base.Rent(position).gameObject;
    }

    public void Return(GameObject go)
    {
        MonsterMover mover = go.GetComponent<MonsterMover>();
        if (mover != null) base.Return(mover);
    }
}
