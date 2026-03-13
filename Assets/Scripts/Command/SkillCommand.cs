public class Skill1Command : ICommand
{
    private readonly IAttackable _attackable;

    public Skill1Command(IAttackable attackable)
    {
        _attackable = attackable;
    }

    public void Execute()
    {
        _attackable.UseSkill(1);
    }
}

public class Skill2Command : ICommand
{
    private readonly IAttackable _attackable;

    public Skill2Command(IAttackable attackable)
    {
        _attackable = attackable;
    }

    public void Execute()
    {
        _attackable.UseSkill(2);
    }
}

public class Skill3Command : ICommand
{
    private readonly IAttackable _attackable;

    public Skill3Command(IAttackable attackable)
    {
        _attackable = attackable;
    }

    public void Execute()
    {
        _attackable.UseSkill(3);
    }
}
