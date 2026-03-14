// 넉백을 받을 수 있는 대상의 계약
// 패턴 클래스는 PlayerStats를 직접 참조하지 않고 이 인터페이스만 사용한다
public interface IKnockbackable
{
    // knockbackSourceX: 넉백을 가한 오브젝트의 x 위치 (방향 계산용)
    // knockbackStrength: 0(넉백 없음) ~ 1(최대 넉백) 정규화 값
    void ApplyKnockback(float knockbackSourceX, float knockbackStrength);
}
