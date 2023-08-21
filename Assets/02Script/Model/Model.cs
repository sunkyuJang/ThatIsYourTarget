using System;
using System.Collections;
using UnityEngine;

public abstract class Model : MonoBehaviour, IDamageController, IObjDetectorConnector_OnContecting, IObjDetectorConnector_OnDetected, IObjDetectorConnector_OnRemoved
{
    public float HP { set { HP -= value; HP = HP < 0 ? 0 : HP; } get { return HP; } }
    public int state { private set; get; } = 0;
    public ModelPhysicsHandler modelPhysicsHandler { protected set; get; }
    Transform APHGroup;
    AnimationPointHandler originalAPH;
    Action nextActionFromState = null;
    float MaxAcceptableDmgTime { set; get; } = 2f;
    bool CanAcceptableDmg { set; get; } = true;
    JobManager jobManager { set; get; }
    public StateModuleHandler moduleHandler { protected set; get; }
    protected virtual void Awake()
    {
        modelPhysicsHandler = GetComponentInChildren<ModelPhysicsHandler>();
        APHGroup = transform.Find("APHGroup");
        originalAPH = APHGroup.Find("OriginalAPH").GetComponent<AnimationPointHandler>();
        jobManager = new JobManager(GetNextAPH);
        moduleHandler = SetStateModuleHandler();
    }
    protected abstract StateModuleHandler SetStateModuleHandler();
    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => APHManager.Instance.IsReady);
        SetOriginalAPH();
    }
    protected virtual void SetState(int newState, StateModule.PrepareData prepareData = null)
    {
        moduleHandler.EnterModule(newState, prepareData);
    }
    public void SetOriginalAPH()
    {
        SetAPH(originalAPH);
    }

    public void ReturnAPH(AnimationPointHandler APH)
    {
        if (APH != originalAPH)
            APHManager.Instance.ReturnAPH(APH);
    }
    public void SetAPH(AnimationPointHandler handler, Action nextActionFromState = null)
    {
        if (jobManager == null) jobManager = new JobManager(GetNextAPH);
        var job = new ModelJob(jobManager, handler, ReturnAPH);
        job.jobAction = () => (modelPhysicsHandler as IJobStarter<ModelJob>).StartJob(job);
        jobManager.AddJob(job);
        jobManager.StartJob();
        if (nextActionFromState != null) this.nextActionFromState = nextActionFromState;
    }

    protected virtual void EndEachJob()
    {
        GetNextAPH();
    }
    protected virtual void ExceptionJob()
    {
        print("somthing wrong with ModelJob");
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
    public void OnContecting(ObjDetector detector, Collider collider) => OnContecting(collider);
    public void OnDetected(ObjDetector detector, Collider collider) => OnDetected(collider);
    public void OnRemoved(ObjDetector detector, Collider collider) => OnRemoved(collider);
    public virtual void OnContecting(Collider collider) { }
    public virtual void OnDetected(Collider collider) { }
    public virtual void OnRemoved(Collider collider) { }
    protected virtual void ChangedState() { }
    protected virtual void DoDie() { }

    public bool SetDamage(float damege)
    {
        if (CanAcceptableDmg)
        {
            CanAcceptableDmg = false;
            Action removeTimeData = () => { CanAcceptableDmg = true; };
            TimeCounter.Instance.SetTimeCounting(MaxAcceptableDmgTime, removeTimeData);

            HP = damege;

            if (HP <= 0)
            {
                DoDie();
            }
        }

        return false;
    }

    public class ModelJob : Job
    {
        public AnimationPointHandler aph { private set; get; }
        public Action<AnimationPointHandler> returnAPH { private set; get; }
        public ModelJob(JobManager jobManager, AnimationPointHandler aph, Action<AnimationPointHandler> returnAPH) : base(jobManager)
        {
            this.aph = aph;
            this.returnAPH = returnAPH;
        }
    }
}
