using UnityEngine;

public abstract class StateModule
{
    protected bool isStateRunning = false;
    public abstract bool IsReadyForEnter();
    public abstract void Enter();
    public abstract void EnterToException();
    public virtual void AfterAPHDone() { }
    public abstract void Exit();
    protected class TrackingData
    {
        public Transform target = null;
        public bool isFollowing = false;
        public bool shouldRemove = false;
        public bool CanRemove { get { return !isFollowing && shouldRemove; } }
        public TrackingData(Transform target)
        {
            this.target = target;
        }
    }
}
