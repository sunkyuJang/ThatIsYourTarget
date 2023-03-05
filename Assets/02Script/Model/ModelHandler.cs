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
    public Queue<Action> jobList = new Queue<Action>();

    private void Awake()
    {
        model = GetComponentInParent<Model>();
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();
        ragDollHandler = GetComponent<RagDollHandler>();

        naviJobStarter = naviController as IJobStarter;
        aniJobStarter = aniController as IJobStarter;
    }
    public void SetAPH(ActionPointHandler handler)
    {
        if (actionPointHandler != null)
            model.ReturnAPH(actionPointHandler);

        actionPointHandler = handler;
        SetJobForAPHRead();
    }

    void SetNowAPPosition()
    {
        var nowAP = actionPointHandler.GetNowActionPoint();
        new ModelJob(naviJobStarter, nowAP, StartNextJob, SetExceptionByNavi).StartJob();
    }

    void SetJobForAPHRead()
    {
        jobList.Clear();
        jobList.Enqueue(SetNowAPPosition);
        jobList.Enqueue(ReadAP);
        StartNextJob();
    }

    void StartNextJob()
    {
        jobList.Dequeue().Invoke();
    }

    private void ReadAP()
    {
        var nowAP = actionPointHandler.GetNowActionPoint();
        if (nowAP.HasAction)
        {
            new ModelJob(aniJobStarter, nowAP, ReadNextAction, SetExceptionByAni).StartJob();
        }
        else
        {
            ReadNextAction();
        }
    }

    public void ReadNextAction()
    {
        if (actionPointHandler.isAPHDone)
        {
            model.GetNextAPH();
        }
        else
        {
            actionPointHandler.GetNextActionPoint();
            SetJobForAPHRead();
        }
    }
    void SetExceptionByNavi() { }
    void SetExceptionByAni() { }

    public void OnRemoved(ObjDetector detector, Collider collider)
    {
        model.Removed(collider);
    }

    public void OnDetected(ObjDetector detector, Collider collider)
    {
        model.Contected(collider);
    }
    public class ModelJob : Job
    {
        public ActionPoint ap { private set; get; }
        public ModelJob(IJobStarter starter, ActionPoint ap, Action endAction, Action exceptionAction)
        {
            this.jobStarter = starter;
            this.endAction = endAction;
            this.exceptionAction = exceptionAction;
            this.ap = ap;
        }
    }
}