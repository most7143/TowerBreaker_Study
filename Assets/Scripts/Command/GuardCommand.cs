public class GuardCommand : ICommand
{
    private readonly IGuardable _guardable;

    public GuardCommand(IGuardable guardable)
    {
        _guardable = guardable;
    }

    public void Execute()
    {
        _guardable.GuardStart();
    }
}
