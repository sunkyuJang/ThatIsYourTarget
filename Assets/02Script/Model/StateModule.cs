using UnityEngine;

public abstract class StateModule
{
    protected bool isStateRunning = false;
    public abstract bool IsReadyForEnter();
    public abstract void Enter();
    public abstract void EnterToException();
    public abstract void Exit();
}
