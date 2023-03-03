using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModelHandler : MonoBehaviour, IObjDetectorConnector_OnDetected, IObjDetectorConnector_OnRemoved
{
    Model model;
    public ActionPointHandler actionPointHandler { private set; get; }
    public NaviController naviController { private set; get; }
    public AniController aniController { private set; get; }
    IJobStarter naviJobStarter;
    IJobStarter aniJobStarter;
    RagDollHandler ragDollHandler { set; get; }

    private void Awake()
    {
        model = GetComponentInParent<Model>();
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();
        ragDollHandler = GetComponent<RagDollHandler>();

        naviJobStarter = naviController as IJobStarter;
        aniJobStarter = aniJobStarter as IJobStarter;
    }
    public void SetAPH(ActionPointHandler handler)
    {
        if (actionPointHandler != null)
            model.ReturnAPH(actionPointHandler);

        actionPointHandler = handler;

        var jobList = new List<Action>();
        jobList.Add(SetNextTargetPosition);
        jobList.Add(ReadNextAction);

    }
    public void ChageLastAPPosition(Transform target)
    {
        var lastAP = actionPointHandler.GetActionPoint(actionPointHandler.GetActionCount - 1);
        lastAP.SetPositionForTracking(transform, target, true);
    }

    void SetNextTargetPosition()
    {
        var naviJob = new NaviController.NaviJob(naviJobStarter, actionPointHandler.GetNowActionPoint(), StartNextJob, SetException);
    }

    void StartNextJob()
    {

    }
    void SetException()
    {

    }

    private void ReadNowAction()
    {
        var ap = actionPointHandler.GetNowActionPoint();
        if (ap.HasAction)
        {
            aniJobStarter.StartJob(ap);
        }
        else
        {
            ReadNextAction();
        }
    }

    public void ReadNextAction()
    {
        if (actionPointHandler.IsReachedToEnd)
        {
            model.GetNextAPH();
        }
        else
        {
            SetNextTargetPosition();
        }
    }

    public void OnRemoved(ObjDetector detector, Collider collider)
    {
        model.Removed(collider);
    }

    public void OnDetected(ObjDetector detector, Collider collider)
    {
        model.Contected(collider);
    }

    // class ModelJobStater
    // {
    //     IModelHandlerJobStarter targetJobStarter;
    //     Queue<Action> jobs = new Queue<Action>();
    //     Action endAction = null;
    //     Action exceptionAction = null;
    //     List<T> requiredOption = new List<T>();
    //     public ModelJobStater(IModelHandlerJobStarter jobStarter, List<Action> jobs, Action endAction, Action exceptionAction, List<T> requiredOption)
    //     {
    //         targetJobStarter = jobStarter;
    //         jobs.ForEach(x => this.jobs.Enqueue(x));
    //         this.endAction = endAction;
    //         this.exceptionAction = exceptionAction;
    //         this.requiredOption = requiredOption;
    //     }

    //     public void StartJob()
    //     {
    //         targetJobStarter.StartJob(requiredOption, endAction, exceptionAction);
    //     }
    // }
}