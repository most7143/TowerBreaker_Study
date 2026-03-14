// 넉백 세기 최댓값 — 0~1 정규화 값에 곱해 실제 속도를 계산
public static class KnockbackValues
{
    public const float MaxForce = 220f;

    // normalizedStrength: Inspector에서 설정한 0~1 값
    public static float Get(float normalizedStrength) => normalizedStrength * MaxForce;
}
