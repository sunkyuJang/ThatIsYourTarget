using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModelHandler : MonoBehaviour, IJobStarter, ISectionJobChecker, IObjDetectorConnector_OnDetected, IObjDetectorConnector_OnRemoved
{
    Model model;
    public ActionPointHandler actionPointHandler { private set; get; }
    public NaviController naviController { private set; get; }
    public AniController aniController { private set; get; }
    IJobStarter naviJobStarter;
    IJobStarter aniJobStarter;
    RagDollHandler ragDollHandler { set; get; }
    Model.ModelJob modelJob { set; get; }
    public Queue<Action> jobList = new Queue<Action>();

    enum JobKind { naviJob, aniJob, nextJob, SetNextJob, doneJob, non }
    private void Awake()
    {
        model = GetComponentInParent<Model>();
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();
        ragDollHandler = GetComponent<RagDollHandler>();

        naviJobStarter = naviController as IJobStarter;
        aniJobStarter = aniController as IJobStarter;
    }
    public void StartJob(Job job)
    {
        if (job is Model.ModelJob)
        {
            modelJob = job as Model.ModelJob;
            SetAPH(modelJob.aph);
        }
    }
    public void SetAPH(ActionPointHandler handler)
    {
        if (actionPointHandler != null)
            model.ReturnAPH(actionPointHandler);

        actionPointHandler = handler;
        CreatingJob(modelJob);
        EndEachJob();
    }

    void CreatingJob(Model.ModelJob sectionJob)
    {
        jobList.Clear();

        var nowAP = actionPointHandler.GetNowActionPoint();
        var isAPHDone = actionPointHandler.isAPHDone;
        var state = isAPHDone ? (int)JobKind.doneJob : 0;
        for (int i = state; i < (int)JobKind.non; i++)
        {
            Action action = null;
            switch ((JobKind)i)
            {
                case JobKind.naviJob:
                    {
                        action = GetNewModelHandlerJob(sectionJob, naviJobStarter, EndEachJob, SetExceptionByNavi).StartJob;
                    }
                    break;
                case JobKind.aniJob:
                    {
                        if (nowAP.HasAction)
                            action = GetNewModelHandlerJob(sectionJob, aniJobStarter, EndEachJob, SetExceptionByAni).StartJob;
                        else continue;
                    }
                    break;
                case JobKind.nextJob:
                    {
                        action = ReadNextAP;
                    }
                    break;
                case JobKind.doneJob:
                    if (isAPHDone)
                        action = DonePersonJob;
                    break;
            }

            if (action != null)
                jobList.Enqueue(action);
        }
    }

    void EndEachJob(Job job = null)
    {
        if (jobList.Count > 0)
            jobList.Dequeue().Invoke();
        else
        {
            DonePersonJob();
        }
    }
    void ReadNextAP()
    {
        actionPointHandler.GetNextActionPoint();
        CreatingJob(modelJob);
    }

    void DonePersonJob()
    {
        modelJob.EndJob();
    }

    void SetExceptionByNavi(Job job) { }
    void SetExceptionByAni(Job job) { }

    public void OnRemoved(ObjDetector detector, Collider collider)
    {
        model.Removed(collider);
    }

    public void OnDetected(ObjDetector detector, Collider collider)
    {
        model.Contected(collider);
    }

    public ModelHandlerJob GetNewModelHandlerJob(Job job, IJobStarter jobStarter, Action<Job> endAction, Action<Job> exceptionAction)
    {
        var ap = actionPointHandler.GetNowActionPoint();
        var sectionChecker = this as ISectionJobChecker;
        return new ModelHandlerJob(sectionChecker, job, jobStarter, ap, endAction, exceptionAction);
    }

    public bool IsSameSection(Job job)
    {
        return modelJob.Equals(job);
    }

    public class ModelHandlerJob : SectionJob
    {
        public ActionPoint ap { private set; get; }
        public ModelHandlerJob(ISectionJobChecker sectionChecker, Job job, IJobStarter starter, ActionPoint ap, Action<Job> endAction, Action<Job> exceptionAction)
                : base(job, starter, sectionChecker, endAction, exceptionAction)
        {
            this.ap = ap;
        }
    }
}