using UnityEngine;
using UnityEngine.UI;

// 플레이어 HP를 이미지 3개로 시각화
// - HP만큼 이미지 활성화, 피해를 입으면 오른쪽부터 하나씩 비활성화
public class PlayerHpUI : MonoBehaviour
{
    public static PlayerHpUI Instance { get; private set; }

    // Inspector에서 HP 이미지 3개를 순서대로 연결 (인덱스 0 = 첫 번째 체력)
    [SerializeField] private Image[] hpImages;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateHp(hpImages.Length);
    }

    // currentHp에 맞게 이미지를 켜고 끔
    public void UpdateHp(int currentHp)
    {
        for (int i = 0; i < hpImages.Length; i++)
            hpImages[i].gameObject.SetActive(i < currentHp);
    }
}
