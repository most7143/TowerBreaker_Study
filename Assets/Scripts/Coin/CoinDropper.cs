using UnityEngine;

// 몬스터 사망 위치에서 코인을 튀어오르게 생성
public class CoinDropper : MonoBehaviour
{
    [SerializeField] private float peakHeight = 1.5f;   // 포물선 최고점 높이
    [SerializeField] private float arcDuration = 0.6f;  // 솟아오르고 착지까지 걸리는 시간
    [SerializeField] private float spreadX = 0.8f;      // 착지 X 분산 범위 (±)
    [SerializeField] private float fallDepth = 5f;      // 스폰 위치 기준 얼마나 아래로 향할지 (Ground 충돌이 먼저 멈춤)

    // MonsterStats.OnDiedWithCoin에 등록 - (사망 위치, StageData의 coinPerMonster)
    public void Drop(Vector3 position, int coinCount)
    {
        GoldWallet.Instance?.Add(coinCount);

        for (int i = 0; i < coinCount; i++)
        {
            float landOffsetX = Random.Range(-spreadX, spreadX);
            Vector3 spawnPos = new Vector3(position.x + landOffsetX * 0.15f, position.y, position.z);
            Vector3 landPos = new Vector3(position.x + landOffsetX, position.y - fallDepth, position.z);

            Coin coin = CoinPool.Instance.Rent(spawnPos);
            coin.Launch(spawnPos, landPos, peakHeight, arcDuration);
        }
    }
}
