using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public int state { private set; get; } = 0;
    protected ModelHandler modelHandler;
    Transform APHGroup;
    ActionPointHandler originalAPH;

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
            print(state + "in");
            ChangedState(state);
        }
        print(state + "out");
    }
    public void SetOriginalAPH()
    {
        SetAPH(originalAPH);
    }

    private void Update()
    {
        if (true)
        {
            var sd = 10;
        }
    }

    public void ReturnAPH(ActionPointHandler APH)
    {
        if (APH != originalAPH)
            APHManager.Instance.ReturnAPH(APH);
    }
    public void SetAPH(ActionPointHandler handler)
    {
        modelHandler.SetAPH(handler);
    }
    public void StartToAPHRead() { modelHandler.ReadNextAction(); }
    public virtual void Contected(Collider collider) { }
    public virtual void Contecting(Collider collider) { }
    public virtual void Removed(Collider collider) { }
    public virtual void ChangedState(int i) { }
    public virtual void GetHit() { }
}
