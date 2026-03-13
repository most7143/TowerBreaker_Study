using UnityEngine;

// 버튼 입력을 커맨드로 변환하여 실행하는 진입점
public class PlayerController : MonoBehaviour
{
    private PlayerAttack _playerAttack;
    private IDashable _dashable;
    private IGuardable _guardable;

    private ICommand _dashCommand;
    private ICommand _skill1Command;
    private ICommand _skill2Command;
    private ICommand _skill3Command;

    private void Awake()
    {
        _playerAttack = GetComponent<PlayerAttack>();
        _dashable = GetComponent<PlayerDash>();
        _guardable = GetComponent<PlayerGuard>();

        _dashCommand = new DashCommand(_dashable);
        _skill1Command = new Skill1Command(_playerAttack);
        _skill2Command = new Skill2Command(_playerAttack);
        _skill3Command = new Skill3Command(_playerAttack);
    }

    private void Update()
    {
        // 공격: D키 (PointerDown/Up 방식 그대로 재현)
        if (Input.GetKeyDown(KeyCode.D)) OnAttackButtonDown();
        if (Input.GetKeyUp(KeyCode.D)) OnAttackButtonUp();

        // 대시: A키
        if (Input.GetKeyDown(KeyCode.A)) OnDashButton();

        // 가드: S키 (hold 방식)
        if (Input.GetKeyDown(KeyCode.S)) OnGuardButtonDown();
        if (Input.GetKeyUp(KeyCode.S)) OnGuardButtonUp();

        // 스킬: Q / W / E키
        if (Input.GetKeyDown(KeyCode.Q)) OnSkill1Button();
        if (Input.GetKeyDown(KeyCode.W)) OnSkill2Button();
        if (Input.GetKeyDown(KeyCode.E)) OnSkill3Button();
    }

    // 공격 버튼: EventTrigger의 PointerDown/PointerUp에 연결
    public void OnAttackButtonDown() => _playerAttack.AttackButtonDown();
    public void OnAttackButtonUp() => _playerAttack.AttackButtonUp();

    // 단발 호출용 (기존 OnClick 버튼 등에서 사용 가능)
    public void OnAttackButton() => _playerAttack.Attack();

    // 대시·스킬은 공격 중에도 사용 가능
    public void OnDashButton() => _dashCommand.Execute();
    public void OnSkill1Button() => _skill1Command.Execute();
    public void OnSkill2Button() => _skill2Command.Execute();
    public void OnSkill3Button() => _skill3Command.Execute();

    // 가드 버튼: EventTrigger의 PointerDown/PointerUp에 연결
    public void OnGuardButtonDown() => _guardable.GuardStart();
    public void OnGuardButtonUp() => _guardable.GuardEnd();
}
