using UnityEngine;

// 코인 오브젝트 풀
public class CoinPool : ObjectPool<Coin>
{
    public static CoinPool Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    protected override void OnReturn(Coin coin)
    {
        coin.StopAllCoroutines();
    }
}
