using UnityEngine;

public class PlayerStats : CharacterBase
{
    protected override void Awake()
    {
        maxHp = 3;
        base.Awake();
    }

    protected override void OnDamaged(int amount)
    {
        Debug.Log($"Player HP: {currentHp}/{maxHp}");
        PlayerHpUI.Instance?.UpdateHp(currentHp);
    }

    protected override void OnDead()
    {
        Debug.Log("게임 오버");
        PlayerHpUI.Instance?.UpdateHp(0);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }
}
