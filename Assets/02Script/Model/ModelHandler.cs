using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMath;

public class ModelHandler : MonoBehaviour, IJobStarter, ISectionJobChecker
{
    public ActionPointHandler actionPointHandler { private set; get; }
    public IJobStarter naviJobStarter { private set; get; }
    public IJobStarter aniJobstarter { private set; get; }
    RagDollHandler ragDollHandler { set; get; }
    Model.ModelJob modelJob { set; get; }
    ModelHandlerJobManager modelHandlerJobManager { set; get; }

    private void Awake()
    {
        ragDollHandler = GetComponent<RagDollHandler>();
        var naviController = GetComponent<NaviController>();
        var aniController = GetComponent<AniController>();

        naviJobStarter = CastingAsIJobStarter<NaviController>(naviController);
        aniJobstarter = CastingAsIJobStarter<AniController>(aniController);
    }

    private IJobStarter CastingAsIJobStarter<T>(T target)
    {
        if (target != null)
        {
            return target as IJobStarter;
        }

        return null;
    }

    public void StartJob(Job job)
    {
        if (job is Model.ModelJob)
        {
            modelJob = job as Model.ModelJob;

            if (actionPointHandler != null)
            {
                modelJob.recycleAPHFunc(actionPointHandler);
            }

            if (modelHandlerJobManager != null)
                modelHandlerJobManager.StopRunning();

            StopJob();

            actionPointHandler = modelJob.aph;
            modelHandlerJobManager = new ModelHandlerJobManager
                                        (
                                            doEndJob: DonePersonJob,
                                            naviJobStarter: naviJobStarter,
                                            aniJobStarter: aniJobstarter,
                                            parentJob: modelJob,
                                            aph: actionPointHandler,
                                            sectionJobChecker: this as ISectionJobChecker
                                        );
            modelHandlerJobManager.StartJob();
        }
    }

    public void StopJob()
    {
        naviJobStarter.StopJob();
        aniJobstarter.StopJob();
    }

    void DonePersonJob()
    {
        modelJob.EndJob();
    }

    public bool IsSameSection(Job job)
    {
        return modelJob.Equals(job);
    }

    public class ModelHandlerJob : SectionJob
    {
        public ActionPoint ap { private set; get; }
        public ActionPointHandler.WalkingState walkingState { private set; get; }
        public ModelHandlerJob(ISectionJobChecker sectionChecker, ActionPointHandler.WalkingState walkingState, ActionPoint ap, Job job, IJobStarter starter, Action endAction, Action exceptionAction)
                    : base(job, starter, sectionChecker, endAction, exceptionAction)
        {
            this.walkingState = walkingState;
            this.ap = ap;
        }
    }

    public float GetDistTo(Transform target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public RaycastHit[] GetAllRayHIts(Transform target)
    {
        var from = transform.position;
        var to = target.position;
        var dir = Vector3Extentioner.GetDirection(from, to);
        var dist = Vector3.Distance(from, to);

        return Physics.RaycastAll(from, dir, dist, 0, QueryTriggerInteraction.Ignore);
    }

    public void SetDead()
    {
        StopJob();
        ragDollHandler.TrunOnRigid(true);
    }
}