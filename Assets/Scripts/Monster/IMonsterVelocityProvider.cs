using UnityEngine;

// MonsterContactHandler가 몬스터 속도를 조회할 때 사용하는 인터페이스
// MonsterGroup의 구체 타입을 직접 참조하지 않아도 됨
public interface IMonsterVelocityProvider
{
    Rigidbody2D[] GetAllRigidbodies();
}
