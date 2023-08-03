public abstract class StateModule
{
    protected bool isStateRunning = false;
    public abstract bool IsReady();
    public bool Enter<T>(T t = null) where T : PrepareData
    {
        if (IsReady())
        {
            StartModule();
            return true;
        }
        return false;
    }
    protected abstract void StartModule();
    public abstract void EnterToException();
    public abstract void Exit();

    public abstract class PrepareData { }
}