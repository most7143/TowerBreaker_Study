using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 라운드 클리어 후 표시되는 강화 선택 팝업
// 5가지 강화 중 3개를 랜덤으로 선택해 표시
public class UpgradePopup : PopupUI
{
    [SerializeField] private UpgradeOptionUI[] optionSlots; // Inspector에 3개 연결
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI statsText;

    public event Action OnClosed;

    private const int OptionCount = 3;

    private PlayerUpgradeState _upgradeState;
    private PlayerStats _playerStats;

    public void Show(PlayerUpgradeState upgradeState, PlayerStats playerStats)
    {
        _upgradeState = upgradeState;
        _playerStats = playerStats;

        UpgradeType[] selected = PickRandom(OptionCount);

        for (int i = 0; i < optionSlots.Length; i++)
            optionSlots[i].Setup(selected[i], this, _upgradeState);

        RefreshGoldText();
        RefreshStatsText();

        if (GoldWallet.Instance != null)
            GoldWallet.Instance.OnGoldChanged += OnGoldChanged;

        Time.timeScale = 0f;
        Open();
    }

    public override void Open()
    {
        base.Open();

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickClose);
    }

    public override void Close()
    {
        if (GoldWallet.Instance != null)
            GoldWallet.Instance.OnGoldChanged -= OnGoldChanged;

        base.Close();
    }

    // UpgradeOptionUI에서 강화 시도 완료 시 호출 — 골드 UI 갱신
    public void OnUpgradeDone()
    {
        RefreshGoldText();
        RefreshStatsText();
    }

    private void OnClickClose()
    {
        Time.timeScale = 1f;

        // 체력 회복 강화가 있으면 실제 HP에 적용
        ApplyHpRecover();

        // 공격 속도 등 강화 수치를 애니메이터에 반영
        _playerStats?.ApplyUpgrades();

        Close();
        OnClosed?.Invoke();
    }

    private void ApplyHpRecover()
    {
        if (_upgradeState == null || _playerStats == null) return;
        if (_upgradeState.HpRecovered <= 0) return;

        int amount = _upgradeState.HpRecovered;
        _playerStats.Heal(amount);

        // 소비 후 초기화 (중복 적용 방지)
        _upgradeState.ConsumeHpRecover();
    }

    private void RefreshGoldText()
    {
        if (goldText != null && GoldWallet.Instance != null)
            goldText.text = $"보유 골드: {GoldWallet.Instance.Gold}";
    }

    private void RefreshStatsText()
    {
        if (statsText == null || _upgradeState == null) return;

        PlayerStatsData baseData = Resources.Load<PlayerStatsData>("PlayerData/PlayerStatsData");
        if (baseData == null) return;

        int hp = baseData.maxHp;
        int atk = baseData.attackPower + _upgradeState.AttackPowerBonus;
        float atkSpd = baseData.attackSpeed + _upgradeState.AttackSpeedBonus;
        float guardCD = Mathf.Max(0f, baseData.guardCooldown - _upgradeState.GuardCooldownReduction);
        float guardForce = baseData.guardPushForce + _upgradeState.GuardPushForceBonus;

        statsText.text =
            $"HP: {hp}\n" +
            $"공격력: {atk}\n" +
            $"공격 속도: {atkSpd:F1}\n" +
            $"가드 쿨타임: {guardCD:F1}초\n" +
            $"가드 넉백: {guardForce:F1}";
    }

    private void OnGoldChanged(int newGold)
    {
        if (goldText != null)
            goldText.text = $"보유 골드: {newGold}";
    }

    // 전체 UpgradeType 중 count개를 중복 없이 랜덤 선택
    private static UpgradeType[] PickRandom(int count)
    {
        UpgradeType[] all = (UpgradeType[])Enum.GetValues(typeof(UpgradeType));

        // Fisher-Yates shuffle
        for (int i = all.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (all[i], all[j]) = (all[j], all[i]);
        }

        UpgradeType[] result = new UpgradeType[count];
        Array.Copy(all, result, count);
        return result;
    }
}
