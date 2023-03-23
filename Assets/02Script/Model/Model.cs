using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour, IDamageController
{
    public int HP { set; get; } = 10;
    public int state { private set; get; } = 0;
    public ModelHandler modelHandler { private set; get; }
    Transform APHGroup;
    ActionPointHandler originalAPH;
    Action nextActionFromState = null;
    float MaxAcceptableDmgTime { set; get; } = 2f;
    bool CanAcceptableDmg { set; get; } = true;
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
            ChangedState();
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
        new ModelJob(handler, modelHandler as IJobStarter, EndEachJob, ExceptionJob).StartJob();
        if (nextActionFromState != null) this.nextActionFromState = nextActionFromState;
    }

    protected virtual void EndEachJob()
    {
        GetNextAPH();
    }
    protected virtual void ExceptionJob()
    {
        GetNextAPH();
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
    public virtual void Contected(Collider collider) { }
    public virtual void Contecting(Collider collider) { }
    public virtual void Removed(Collider collider) { }
    protected virtual void ChangedState() { }

    public void SetDamage(float damege)
    {
        if (CanAcceptableDmg)
        {
            CanAcceptableDmg = false;
            Action removeTimeData = () => { CanAcceptableDmg = true; };
            TimeCounter.Instance.SetTimeCounting(MaxAcceptableDmgTime, removeTimeData);
        }
    }

    public class ModelJob : Job
    {
        public ActionPointHandler aph { private set; get; }
        public ModelJob(ActionPointHandler aph, IJobStarter starter, Action endAction, Action exceptionAction)
                : base(starter, endAction, exceptionAction)
        {
            this.aph = aph;
        }
    }
}
