using UnityEngine;
using System;

// Resources/UpgradeData/UpgradeData.asset 에 위치
// 로드: Resources.Load<UpgradeData>("UpgradeData/UpgradeData")
[CreateAssetMenu(fileName = "UpgradeData", menuName = "TowerBreaker/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("공격력 강화")]
    [Tooltip("강화당 증가하는 공격력")]
    public int attackPowerBonus = 10;
    public int attackPowerGoldCost = 30;
    public UpgradeRateConfig attackPowerRate = new UpgradeRateConfig(0.8f, 0.1f, 0.1f);

    [Header("체력 회복")]
    [Tooltip("강화당 회복하는 체력")]
    public int hpRecoverAmount = 1;
    public int hpRecoverGoldCost = 20;
    public UpgradeRateConfig hpRecoverRate = new UpgradeRateConfig(0.8f, 0.1f, 0.1f);

    [Header("공격 속도 증가")]
    [Tooltip("강화당 증가하는 공격 속도 (Animator AttackSpeed 파라미터)")]
    public float attackSpeedBonus = 0.2f;
    public int attackSpeedGoldCost = 25;
    public UpgradeRateConfig attackSpeedRate = new UpgradeRateConfig(0.8f, 0.1f, 0.1f);

    [Header("가드 쿨타임 감소")]
    [Tooltip("강화당 감소하는 가드 쿨타임 (초)")]
    public float guardCooldownReduction = 0.3f;
    public int guardCooldownGoldCost = 25;
    public UpgradeRateConfig guardCooldownRate = new UpgradeRateConfig(0.8f, 0.1f, 0.1f);

    [Header("가드 넉백 강화")]
    [Tooltip("강화당 증가하는 가드 넉백 힘")]
    public float guardPushForceBonus = 2f;
    public int guardPushForceGoldCost = 30;
    public UpgradeRateConfig guardPushForceRate = new UpgradeRateConfig(0.8f, 0.1f, 0.1f);

    public UpgradeRateConfig GetRateConfig(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.AttackPower    => attackPowerRate,
            UpgradeType.HpRecover      => hpRecoverRate,
            UpgradeType.AttackSpeed    => attackSpeedRate,
            UpgradeType.GuardCooldown  => guardCooldownRate,
            UpgradeType.GuardPushForce => guardPushForceRate,
            _ => attackPowerRate
        };
    }
}

[Serializable]
public struct UpgradeRateConfig
{
    [Tooltip("첫 번째 강화 시도 성공 확률 (0~1)")]
    public float initialSuccessRate;
    [Tooltip("강화 성공 후 확률 감소량 (0~1)")]
    public float successRateDecrement;
    [Tooltip("최소 성공 확률 (0~1)")]
    public float minimumSuccessRate;

    public UpgradeRateConfig(float initial, float decrement, float minimum)
    {
        initialSuccessRate  = initial;
        successRateDecrement = decrement;
        minimumSuccessRate  = minimum;
    }
}
