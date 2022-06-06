using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public int state { private set; get; } = 0;
    protected ModelPhysicsController modelPhysicsController;
    Transform APHGroup;
    ActionPointHandler originalAPH;

    protected virtual void Awake()
    {
        modelPhysicsController = GetComponentInChildren<ModelPhysicsController>();
        APHGroup = transform.Find("APHGroup");
        originalAPH = APHGroup.Find("OriginalAPH").GetComponent<ActionPointHandler>();
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
    public void SetOriginalAPH() => SetAPH(APHManager.Instance.GetCopyAPH(originalAPH));
    public void SetAPH(ActionPointHandler handler)
    {
        handler.transform.SetParent(APHGroup);
        modelPhysicsController.SetAPH(handler);
    }
    public void StartToAPHRead() { modelPhysicsController.ReadNextAction(); }
    public virtual void Contected(Collider collider) { }
    public virtual void Contecting(Collider collider) { }
    public virtual void Removed(Collider collider) { }
    public virtual void ChangedState(int i) { }
    public virtual void GetHit() { }
}
