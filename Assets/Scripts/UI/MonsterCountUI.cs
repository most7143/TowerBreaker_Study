using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 스테이지 몬스터 수를 사각형 아이콘으로 시각화
// - 스폰 시 스폰 수만큼 아이콘 생성
// - 몬스터 사망 시 해당 아이콘 숨김
public class MonsterCountUI : MonoBehaviour
{
    public static MonsterCountUI Instance { get; private set; }

    [SerializeField] private Vector2 iconSize = new Vector2(30f, 30f);
    [SerializeField] private float iconSpacing = 8f;
    [SerializeField] private Color iconColor = Color.red;

    private readonly List<Image> _icons = new List<Image>();
    private int _nextHideIndex;

    private void Awake()
    {
        Instance = this;
    }

    // MonsterSpawner에서 스폰 직후 호출
    public void Initialize(int count)
    {
        // 이전 아이콘 제거 (재사용 대비)
        foreach (var icon in _icons)
            Destroy(icon.gameObject);
        _icons.Clear();
        _nextHideIndex = 0;

        for (int i = 0; i < count; i++)
        {
            GameObject iconGo = new GameObject($"MonsterIcon_{i}", typeof(RectTransform), typeof(Image));
            iconGo.transform.SetParent(transform, false);

            RectTransform rt = iconGo.GetComponent<RectTransform>();
            rt.sizeDelta = iconSize;
            // 왼쪽 정렬 기준으로 X 위치 설정
            rt.anchoredPosition = new Vector2(i * (iconSize.x + iconSpacing), 0f);

            Image img = iconGo.GetComponent<Image>();
            img.color = iconColor;

            _icons.Add(img);
        }
    }

    // 몬스터 한 마리 사망 시 호출 - 왼쪽(처음)부터 숨김
    public void HideOne()
    {
        if (_nextHideIndex >= _icons.Count) return;

        _icons[_nextHideIndex].gameObject.SetActive(false);
        _nextHideIndex++;
    }
}
