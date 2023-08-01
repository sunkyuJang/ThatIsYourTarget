public abstract class StateModule
{
    protected bool isStateRunning = false;
    public abstract bool IsReadyForEnter();
    public void Enter()
    {
        DoEnter();
    }
    protected abstract void DoEnter();
    public abstract void EnterToException();
    public abstract void Exit();

    public abstract class PrepareData { }
}