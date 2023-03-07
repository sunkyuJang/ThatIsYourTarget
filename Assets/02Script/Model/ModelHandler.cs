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
    ModelHandlerJobManager modelHandlerJobManager { set; get; }

    private void Awake()
    {
        model = GetComponentInParent<Model>();
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();
        ragDollHandler = GetComponent<RagDollHandler>();
    }
    public void StartJob(Job job)
    {
        if (job is Model.ModelJob)
        {
            modelJob = job as Model.ModelJob;

            if (actionPointHandler != null)
                model.ReturnAPH(actionPointHandler);

            if (modelHandlerJobManager != null)
                modelHandlerJobManager.StopRunning();

            StopJob();

            actionPointHandler = modelJob.aph;
            var naviJobStarter = naviController as IJobStarter;
            var aniJobStarter = aniController as IJobStarter;
            modelHandlerJobManager = new ModelHandlerJobManager(DonePersonJob, naviJobStarter, aniJobStarter, modelJob, actionPointHandler, this as ISectionJobChecker);
            modelHandlerJobManager.StartJob();
        }
    }

    public void StopJob()
    {
        (naviController as IJobStarter).StopJob();
        (aniController as IJobStarter).StopJob();
    }

    void DonePersonJob()
    {
        modelJob.EndJob();
    }

    public void OnRemoved(ObjDetector detector, Collider collider)
    {
        model.Removed(collider);
    }

    public void OnDetected(ObjDetector detector, Collider collider)
    {
        model.Contected(collider);
    }

    public bool IsSameSection(Job job)
    {
        return modelJob.Equals(job);
    }

    public class ModelHandlerJob : SectionJob
    {
        public ActionPoint ap { private set; get; }
        public ModelHandlerJob(ISectionJobChecker sectionChecker, Job job, IJobStarter starter, ActionPoint ap, Action endAction, Action exceptionAction)
                : base(job, starter, sectionChecker, endAction, exceptionAction)
        {
            this.ap = ap;
        }
    }
}