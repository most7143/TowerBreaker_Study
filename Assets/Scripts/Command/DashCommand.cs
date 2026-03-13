public class DashCommand : ICommand
{
    private readonly IDashable _dashable;

    public DashCommand(IDashable dashable)
    {
        _dashable = dashable;
    }

    public void Execute()
    {
        _dashable.Dash();
    }
}
