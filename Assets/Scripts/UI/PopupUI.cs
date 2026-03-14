using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupUI : MonoBehaviour
{
    [SerializeField] private float openDuration = 0.3f;
    [SerializeField] private float slideOffset = 30f; // 시작 위치가 원래보다 아래로 얼마나 내려갈지 (px)
    [SerializeField] private Image backGroundImage;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector2 _originPosition;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _rectTransform = GetComponent<RectTransform>();
        _originPosition = _rectTransform.anchoredPosition;
    }

    public virtual void Open()
    {
        DOTween.Kill(gameObject);

        gameObject.SetActive(true);

        if (backGroundImage != null)
            backGroundImage.gameObject.SetActive(true);

        // 시작 상태: 반투명 + 아래로 offset
        _canvasGroup.alpha = 0.6f;
        _rectTransform.anchoredPosition = _originPosition - new Vector2(0f, slideOffset);

        Sequence seq = DOTween.Sequence().SetTarget(gameObject).SetUpdate(true);
        seq.Join(_canvasGroup.DOFade(1f, openDuration).SetEase(Ease.OutQuad));
        seq.Join(_rectTransform.DOAnchorPos(_originPosition, openDuration).SetEase(Ease.OutQuad));
    }

    public virtual void Close()
    {
        DOTween.Kill(gameObject);

        if (backGroundImage != null)
            backGroundImage.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}