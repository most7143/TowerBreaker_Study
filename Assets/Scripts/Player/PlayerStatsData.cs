using UnityEngine;

// Resources/PlayerData/PlayerStatsData.asset 에 위치
// 로드: Resources.Load<PlayerStatsData>("PlayerData/PlayerStatsData")
[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "TowerBreaker/Player Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    [Header("전투")]
    public int maxHp = 3;
    public int attackPower = 20;

    [Header("공격 속도 (높을수록 빠름)")]
    [Tooltip("애니메이터 Attack 레이어의 AttackSpeed 파라미터에 그대로 적용됩니다.")]
    public float attackSpeed = 1f;

    [Header("가드")]
    public float guardCooldown = 2f;
    public float guardPushForce = 12f;
    public float guardPushDrag = 4f;
}
