using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 강화 팝업 내 선택지 카드 하나
public class UpgradeOptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI rateText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI resultText; // 성공 / 실패 표시

    private UpgradeType _type;
    private UpgradePopup _owner;
    private PlayerUpgradeState _upgradeState;

    public void Setup(UpgradeType type, UpgradePopup owner, PlayerUpgradeState upgradeState)
    {
        _type = type;
        _owner = owner;
        _upgradeState = upgradeState;

        Refresh();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnClickUpgrade);
    }

    private void Refresh()
    {
        float rate = _upgradeState != null ? _upgradeState.GetSuccessRate(_type) : 0.8f;
        int cost = _upgradeState != null ? _upgradeState.GetGoldCost(_type) : 0;

        nameText.text = GetTypeName(_type);
        descText.text = GetTypeDesc();
        rateText.text = $"성공률 {Mathf.RoundToInt(rate * 100)}%";
        costText.text = $"골드 {cost}";

        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    private void OnClickUpgrade()
    {
        if (_upgradeState == null || GoldWallet.Instance == null) return;

        int cost = _upgradeState.GetGoldCost(_type);
        if (!GoldWallet.Instance.TrySpend(cost))
        {
            ShowResult("골드 부족!", Color.red);
            return;
        }

        bool success = _upgradeState.TryUpgrade(_type);
        ShowResult(success ? "강화 성공!" : "강화 실패", success ? Color.yellow : Color.gray);

        Refresh();
        _owner.OnUpgradeDone();
    }

    private void ShowResult(string message, Color color)
    {
        if (resultText == null) return;

        resultText.text = message;
        resultText.color = color;
        resultText.gameObject.SetActive(true);
    }

    private static string GetTypeName(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.AttackPower => "공격력 강화",
            UpgradeType.HpRecover => "체력 회복",
            UpgradeType.AttackSpeed => "공격 속도 증가",
            UpgradeType.GuardCooldown => "가드 쿨타임 감소",
            UpgradeType.GuardPushForce => "가드 넉백 강화",
            _ => ""
        };
    }

    private string GetTypeDesc()
    {
        UpgradeData data = Resources.Load<UpgradeData>("UpgradeData/UpgradeData");
        if (data == null) return "";

        return _type switch
        {
            UpgradeType.AttackPower => $"공격력 +{data.attackPowerBonus}",
            UpgradeType.HpRecover => $"체력 +{data.hpRecoverAmount}",
            UpgradeType.AttackSpeed => $"공격속도 +{data.attackSpeedBonus:F1}",
            UpgradeType.GuardCooldown => $"쿨타임 -{data.guardCooldownReduction:F1}초",
            UpgradeType.GuardPushForce => $"넉백력 +{data.guardPushForceBonus:F1}",
            _ => ""
        };
    }
}
