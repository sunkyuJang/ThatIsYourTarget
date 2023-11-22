public abstract class StateModule
{
    public PrepareData prepareData { protected set; get; }
    protected bool isStateRunning = false;
    public abstract bool IsReady();
    public bool TryEnter<T>(T t = null) where T : PrepareData
    {
        prepareData = t;
        if (IsReady())
        {
            StartModule();
            return true;
        }

        prepareData = null;
        return false;
    }
    protected abstract void StartModule();
    public abstract void EnterToException();
    public abstract void Exit();
    public abstract class PrepareData { }
}