using System.Collections.Generic;
using UnityEngine;

// 오브젝트 풀 추상 베이스 — T는 풀링 대상 컴포넌트 타입
// 서브클래스는 prefab/initialSize를 Inspector로 설정하고
// OnRent / OnReturn 훅을 필요에 따라 오버라이드한다.
public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int initialSize = 8;

    private readonly Queue<T> _pool = new Queue<T>();

    protected virtual void Awake()
    {
        Prewarm();
    }

    // prefab을 교체한 후 풀을 새로 구성할 때 호출
    protected void RebuildPool()
    {
        // 기존 풀 오브젝트 제거
        while (_pool.Count > 0)
        {
            T item = _pool.Dequeue();
            if (item != null)
                Destroy(item.gameObject);
        }
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < initialSize; i++)
        {
            T item = CreateNew();
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }
    }

    private T CreateNew()
    {
        GameObject go = Instantiate(prefab, transform);
        go.SetActive(false);
        return go.GetComponent<T>();
    }

    // 풀에서 꺼내 활성화
    public T Rent(Vector3 position)
    {
        T item = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        item.transform.position = position;
        item.gameObject.SetActive(true);
        OnRent(item);
        return item;
    }

    // 풀로 반납 — 비활성화하고 이 오브젝트의 자식으로 이동
    public void Return(T item)
    {
        OnReturn(item);
        item.gameObject.SetActive(false);
        item.transform.SetParent(transform);
        _pool.Enqueue(item);
    }

    // 서브클래스가 필요할 때 오버라이드
    protected virtual void OnRent(T item) { }
    protected virtual void OnReturn(T item) { }
}
