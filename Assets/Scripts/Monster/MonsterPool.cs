using System.Collections.Generic;
using UnityEngine;

// 몬스터 오브젝트 풀 - Instantiate/Destroy 대신 비활성화로 재사용
public class MonsterPool : MonoBehaviour
{
    public static MonsterPool Instance { get; private set; }

    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private int initialSize = 8;

    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject go = CreateNew();
            go.SetActive(false);
            _pool.Enqueue(go);
        }
    }

    private GameObject CreateNew()
    {
        GameObject go = Instantiate(monsterPrefab, transform);
        go.SetActive(false);
        return go;
    }

    // 풀에서 꺼내 활성화
    public GameObject Rent(Vector3 position)
    {
        GameObject go = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        go.transform.position = position;
        go.SetActive(true);
        return go;
    }

    // 풀로 반납 - 비활성화하여 씬에서 사라짐
    public void Return(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(transform);
        _pool.Enqueue(go);
    }
}
