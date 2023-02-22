using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public int state { private set; get; } = 0;
    public ModelHandler modelHandler { private set; get; }
    Transform APHGroup;
    ActionPointHandler originalAPH;
    Action nextActionFromState = null;

    protected virtual void Awake()
    {
        modelHandler = GetComponentInChildren<ModelHandler>();
        APHGroup = transform.Find("APHGroup");
        originalAPH = APHGroup.Find("OriginalAPH").GetComponent<ActionPointHandler>();
        originalAPH.originalOwener = gameObject;
    }
    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => APHManager.Instance.IsReady);
        SetOriginalAPH();
    }
    public void SetState(int newState)
    {
        if (newState != state)
        {
            state = newState;
            ChangedState(state);
        }
    }
    public void SetOriginalAPH()
    {
        SetAPH(originalAPH);
    }

    public void ReturnAPH(ActionPointHandler APH)
    {
        if (APH != originalAPH)
            APHManager.Instance.ReturnAPH(APH);
    }
    public void SetAPH(ActionPointHandler handler, Action nextActionFromState = null)
    {
        modelHandler.SetAPH(handler);
        if (nextActionFromState != null) this.nextActionFromState = nextActionFromState;
    }

    public void GetNextAPH()
    {
        if (nextActionFromState == null)
        {
            SetOriginalAPH();
        }
        else
        {
            nextActionFromState.Invoke();
        }
    }
    public void StartToAPHRead() { modelHandler.ReadNextAction(); }
    public virtual void Contected(Collider collider) { }
    public virtual void Contecting(Collider collider) { }
    public virtual void Removed(Collider collider) { }
    public virtual void ChangedState(int i) { }
    public virtual void GetHit() { }
    protected class CheckingTrackingState
    {
        public Transform target = null;
        public bool isFollowing = false;
        public bool shouldRemove = false;
        public bool CanRemove { get { return !isFollowing && shouldRemove; } }
        public CheckingTrackingState(Transform target)
        {
            this.target = target;
        }
    }
}
