using System.Collections;
using UnityEngine;

// 코인 하나의 동작: 솟아오름 → 착지(Ground 태그 충돌 or 포물선 완료) → 페이드아웃 → 비활성화
[RequireComponent(typeof(SpriteRenderer))]
public class Coin : MonoBehaviour
{
    [SerializeField] private float fadeDelay = 2f;   // 착지 후 페이드 시작까지 대기
    [SerializeField] private float fadeDuration = 0.5f;

    private SpriteRenderer _sr;
    private bool _landed;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // CoinDropper가 호출 - 스폰 위치와 목표 착지 위치를 전달
    public void Launch(Vector3 spawnPos, Vector3 landPos, float peakHeight, float duration)
    {
        _landed = false;
        transform.position = spawnPos;
        gameObject.SetActive(true);

        Color c = _sr.color;
        c.a = 1f;
        _sr.color = c;

        StopAllCoroutines();
        StartCoroutine(ArcRoutine(spawnPos, landPos, peakHeight, duration));
    }

    // 트리거 콜라이더가 Ground 태그 콜라이더에 닿으면 즉시 착지 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_landed) return;
        if (!other.CompareTag("Ground")) return;

        _landed = true;
        StopAllCoroutines();
        // 콜라이더 상단 면 위에 코인을 올려놓기
        float landY = other.bounds.max.y;
        transform.position = new Vector3(transform.position.x, landY, transform.position.z);
        StartCoroutine(AfterLandRoutine());
    }

    private IEnumerator ArcRoutine(Vector3 start, Vector3 land, float peakHeight, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (_landed) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // X: 선형 이동, Y: 포물선
            float x = Mathf.Lerp(start.x, land.x, t);
            float y = start.y + 4f * peakHeight * t * (1f - t) + (land.y - start.y) * t;
            transform.position = new Vector3(x, y, start.z);

            yield return null;
        }

        if (_landed) yield break;
        _landed = true;
        transform.position = land;

        yield return StartCoroutine(AfterLandRoutine());
    }

    private IEnumerator AfterLandRoutine()
    {
        yield return new WaitForSeconds(fadeDelay);
        yield return FadeOutRoutine();
        gameObject.SetActive(false);
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsed = 0f;
        Color c = _sr.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            _sr.color = c;
            yield return null;
        }

        c.a = 0f;
        _sr.color = c;
    }
}
