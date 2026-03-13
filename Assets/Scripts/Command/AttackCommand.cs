public class AttackCommand : ICommand
{
    private readonly IAttackable _attackable;

    public AttackCommand(IAttackable attackable)
    {
        _attackable = attackable;
    }

    public void Execute()
    {
        _attackable.Attack();
    }
}
