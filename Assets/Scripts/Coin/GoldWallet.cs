using System;
using UnityEngine;

// 플레이어 보유 골드 관리 싱글톤 — DontDestroyOnLoad
public class GoldWallet : MonoBehaviour
{
    public static GoldWallet Instance { get; private set; }

    public int Gold { get; private set; }

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Add(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public bool TrySpend(int amount)
    {
        if (Gold < amount) return false;

        Gold -= amount;
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public void ResetGold()
    {
        Gold = 0;
        OnGoldChanged?.Invoke(Gold);
    }
}
