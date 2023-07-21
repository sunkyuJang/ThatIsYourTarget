using JMath;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelHandler : MonoBehaviour, IJobStarter
{
    ActionPointHandler actionPointHandler { set; get; }
    IJobStarter naviJobStarter { set; get; }
    IJobStarter aniJobstarter { set; get; }
    RagDollHandler ragDollHandler { set; get; }
    Model.ModelJob modelJob { set; get; }
    JobManager jobManager { set; get; }
    enum jobState { navi, ani, done, non }
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
                modelJob.returnAPH(actionPointHandler);
            }

            if (jobManager != null)
                jobManager.CancleJob();

            StopJob();

            actionPointHandler = modelJob.aph;
            jobManager = new JobManager(job, EndJob);
            var ap = actionPointHandler.GetNowActionPoint();
            jobManager.AddJob(CreateJobs(modelJob, ap, actionPointHandler));
            jobManager.StartJob();
        }
    }

    void ReadNextAP()
    {
        var ap = actionPointHandler.GetNextActionPoint();
        if (ap != null && !jobManager.shouldCancle)
        {
            jobManager.AddJob(CreateJobs(modelJob, ap, actionPointHandler));
            jobManager.StartJob();
        }
        else
        {
            jobManager.EndJob();
        }
    }

    void EndJob()
    {
        modelJob?.EndJob();
        modelJob = null;
        jobManager = null;
    }

    private Queue<Job> CreateJobs(object section, ActionPoint ap, ActionPointHandler handler)
    {
        var queue = new Queue<Job>();
        for (jobState i = jobState.navi; i < jobState.non; i++)
        {
            var job = new ModelHandlerJob(jobManager, ap, handler.walkingState);
            Action action = null;
            switch (i)
            {
                case jobState.navi:
                    action = () =>
                    {
                        naviJobStarter.StartJob(job);
                    };
                    break;
                case jobState.ani:
                    action = () =>
                    {
                        aniJobstarter.StartJob(job);
                    };
                    break;
                case jobState.done:
                    action = ReadNextAP;
                    break;
            }

            job.jobAction = action;
            queue.Enqueue(job);
        }

        return queue;
    }

    public void StopJob()
    {
        naviJobStarter.StopJob();
        aniJobstarter.StopJob();
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

        return Physics.RaycastAll(from, dir, dist, 0, QueryTriggerInteraction.Ignore).OrderBy(x => x.distance).ToArray();
    }

    public bool IsHitToTarget(Transform target)
    {
        var from = transform.position;
        var to = target.position;
        var dir = Vector3Extentioner.GetDirection(from, to);
        var dist = Vector3.Distance(from, to);

        return Physics.Raycast(from, dir, dist);
    }

    public void SetDead()
    {
        StopJob();
        ragDollHandler.TrunOnRigid(true);
    }

    public class ModelHandlerJob : Job
    {
        public ActionPoint ap { private set; get; }
        public ActionPointHandler.WalkingState walkingState { private set; get; }
        public ModelHandlerJob(JobManager jobManager, ActionPoint ap, ActionPointHandler.WalkingState walkingState) : base(jobManager)
        {
            this.ap = ap;
            this.walkingState = walkingState;
        }
    }
}