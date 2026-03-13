public interface IGuardable
{
    bool IsGuarding { get; }
    void GuardStart();
    void GuardEnd();
    void OnGuardHit();
}
