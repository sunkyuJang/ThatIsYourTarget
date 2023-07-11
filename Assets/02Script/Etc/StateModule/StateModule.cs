using System.Collections.Generic;

public abstract class StateModule
{
    protected bool isStateRunning = false;
    public abstract bool IsReadyForEnter();
    protected internal void Enter()
    {
        if (IsReadyForEnter())
            DoEnter();
        else
            EnterToException();
    }
    protected abstract void DoEnter();
    public abstract void EnterToException();
    public abstract void Exit();
}