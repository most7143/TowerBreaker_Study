using UnityEngine;
using UnityEngine.UI;

// 플레이어 HP를 이미지 3개로 시각화
// - HP만큼 이미지 활성화, 피해를 입으면 오른쪽부터 하나씩 비활성화
public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    // Inspector에서 HP 이미지 3개를 순서대로 연결 (인덱스 0 = 첫 번째 체력)
    [SerializeField] private Image[] hpImages;

    private void Start()
    {
        playerStats.OnHpChanged += UpdateHp;
        UpdateHp(playerStats.CurrentHp, playerStats.MaxHp);
    }

    private void OnDestroy()
    {
        playerStats.OnHpChanged -= UpdateHp;
    }

    private void UpdateHp(int currentHp, int maxHp)
    {
        for (int i = 0; i < hpImages.Length; i++)
            hpImages[i].gameObject.SetActive(i < currentHp);
    }
}
