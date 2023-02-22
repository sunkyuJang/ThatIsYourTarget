using UnityEngine;

public abstract class StateModule
{
    protected bool isStateRunning = false;
    public abstract void Enter();
    protected abstract void Update();
    public abstract void Exit();
    public abstract void AfterAPHDone();
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
