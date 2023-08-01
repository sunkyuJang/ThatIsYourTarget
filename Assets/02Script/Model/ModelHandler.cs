using JMath;
using SensorToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelHandler : MonoBehaviour, IJobStarter
{
    ActionPointHandler actionPointHandler { set; get; }
    NaviController naviController { set; get; }
    AniController aniController { set; get; }
    IJobStarter naviJobStarter { set; get; }
    IJobStarter aniJobstarter { set; get; }
    RagDollHandler ragDollHandler { set; get; }
    Model.ModelJob modelJob { set; get; }
    JobManager jobManager { set; get; }
    [SerializeField]
    FOVCollider FOVCollider { set; get; }
    float SightLength { get { return FOVCollider.Length * FOVCollider.transform.lossyScale.x; } }
    enum jobState { navi, ani, done, non }
    private void Awake()
    {
        ragDollHandler = GetComponent<RagDollHandler>();
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();

        naviJobStarter = CastingAsIJobStarter(naviController);
        aniJobstarter = CastingAsIJobStarter(aniController);

        FOVCollider = transform.Find("Head").GetComponent<FOVCollider>();
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
        if (ap != null)
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

    public RaycastHit[] GetAllRayHIts(Transform target, float dist = 0f)
    {
        var from = transform.position;
        var to = target.position;
        var dir = from.GetDirection(to);
        dist = dist == 0f ? Vector3.Distance(from, to) : dist;

        return Physics.RaycastAll(from, dir, dist, 0, QueryTriggerInteraction.Ignore).OrderBy(x => x.distance).ToArray();
    }

    bool IsHitToTarget(Transform target, float dist = 0f)
    {
        var from = transform.position;
        var to = target.position;
        var dir = from.GetDirection(to);
        dist = dist == 0f ? Vector3.Distance(from, to) : dist;

        Physics.Raycast(from, dir, out RaycastHit hit, dist);
        return hit.transform == target;
    }

    public bool IsInSight(Transform target)
    {
        return IsHitToTarget(target, SightLength);
    }

    public RaycastHit[] GetAllHitInSight(Transform target)
    {
        return GetAllRayHIts(target, SightLength);
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