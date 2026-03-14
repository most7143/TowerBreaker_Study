using UnityEngine;

// 라운드 간 누적 강화 상태 — 씬(스테이지) 내에서만 유지
public class PlayerUpgradeState : MonoBehaviour
{
    // 누적 강화값
    public int AttackPowerBonus { get; private set; }
    public int HpRecovered { get; private set; }
    public float AttackSpeedBonus { get; private set; }
    public float GuardCooldownReduction { get; private set; }
    public float GuardPushForceBonus { get; private set; }

    // 강화 확률 추적 (타입별로 독립 관리)
    private float[] _successRates;

    private UpgradeData _upgradeData;

    private void Awake()
    {
        _upgradeData = Resources.Load<UpgradeData>("UpgradeData/UpgradeData");
        if (_upgradeData == null)
            Debug.LogError("UpgradeData를 Resources/UpgradeData 폴더에서 찾을 수 없습니다.");

        InitRates();
    }

    private void InitRates()
    {
        UpgradeType[] types = (UpgradeType[])System.Enum.GetValues(typeof(UpgradeType));
        _successRates = new float[types.Length];

        for (int i = 0; i < types.Length; i++)
        {
            _successRates[i] = _upgradeData != null
                ? _upgradeData.GetRateConfig(types[i]).initialSuccessRate
                : 0.8f;
        }
    }

    // 현재 타입의 성공 확률 반환 (0~1)
    public float GetSuccessRate(UpgradeType type)
    {
        return _successRates[(int)type];
    }

    // 강화 시도 — 골드 차감은 호출자가 처리, 여기서는 성공 여부와 확률 갱신만
    public bool TryUpgrade(UpgradeType type)
    {
        float rate = _successRates[(int)type];
        bool success = Random.value <= rate;

        if (success)
        {
            ApplyUpgrade(type);
            ReduceRate(type);
        }

        return success;
    }

    // 강화 성공 후 확률 감소
    private void ReduceRate(UpgradeType type)
    {
        if (_upgradeData == null) return;

        UpgradeRateConfig cfg = _upgradeData.GetRateConfig(type);
        int idx = (int)type;
        _successRates[idx] = Mathf.Max(
            cfg.minimumSuccessRate,
            _successRates[idx] - cfg.successRateDecrement
        );
    }

    private void ApplyUpgrade(UpgradeType type)
    {
        if (_upgradeData == null) return;

        switch (type)
        {
            case UpgradeType.AttackPower:
                AttackPowerBonus += _upgradeData.attackPowerBonus;
                break;
            case UpgradeType.HpRecover:
                HpRecovered += _upgradeData.hpRecoverAmount;
                break;
            case UpgradeType.AttackSpeed:
                AttackSpeedBonus += _upgradeData.attackSpeedBonus;
                break;
            case UpgradeType.GuardCooldown:
                GuardCooldownReduction += _upgradeData.guardCooldownReduction;
                break;
            case UpgradeType.GuardPushForce:
                GuardPushForceBonus += _upgradeData.guardPushForceBonus;
                break;
        }
    }

    // 체력 회복값 소비 (UpgradePopup 닫을 때 PlayerStats에 적용 후 호출)
    public void ConsumeHpRecover()
    {
        HpRecovered = 0;
    }

    public int GetGoldCost(UpgradeType type)
    {
        if (_upgradeData == null) return 0;

        return type switch
        {
            UpgradeType.AttackPower    => _upgradeData.attackPowerGoldCost,
            UpgradeType.HpRecover      => _upgradeData.hpRecoverGoldCost,
            UpgradeType.AttackSpeed    => _upgradeData.attackSpeedGoldCost,
            UpgradeType.GuardCooldown  => _upgradeData.guardCooldownGoldCost,
            UpgradeType.GuardPushForce => _upgradeData.guardPushForceGoldCost,
            _ => 0
        };
    }
}
