using JMath;
using SensorToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelHandler : MonoBehaviour, IJobStarter<Model.ModelJob>, IDamageController
{
    public Model Model { private set; get; }
    ActionPointHandler actionPointHandler { set; get; }
    NaviController naviController { set; get; }
    AniController aniController { set; get; }
    RagDollHandler ragDollHandler { set; get; }
    Model.ModelJob modelJob { set; get; }
    ModelJobManager jobManager { set; get; }
    [SerializeField]
    private FOVCollider FOVCollider;
    float SightLength { get { return FOVCollider.Length * FOVCollider.transform.lossyScale.x; } }
    enum jobState { navi, ani, done, non }
    private void Awake()
    {
        Model = transform.parent.GetComponent<Model>();

        ragDollHandler = GetComponent<RagDollHandler>();
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();
    }

    public void StartJob(Model.ModelJob job)
    {
        if (actionPointHandler != null)
        {
            modelJob.returnAPH(actionPointHandler);
        }

        if (jobManager != null)
            jobManager.CancleJob();

        StopJob();

        modelJob = job;
        actionPointHandler = modelJob.aph;
        jobManager = new ModelJobManager(
                        endJob: EndJob,
                        modelJob: modelJob,
                        naviJobStarter: naviController,
                        aniJobstarter: aniController);
        jobManager.StartJob();
    }
    void EndJob()
    {
        modelJob?.EndJob();
        modelJob = null;
        jobManager = null;
    }

    public void StopJob()
    {
        aniController.StopJob();
        naviController.StopJob();
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

    public bool SetDamage(float damege)
    {
        return ((IDamageController)Model).SetDamage(damege);
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

    public class ModelJobManager : JobManager
    {
        private Model.ModelJob modelJob;
        private IJobStarter<ModelHandlerJob> naviJobStarter;
        private IJobStarter<ModelHandlerJob> aniJobstarter;

        public ModelJobManager(
                Action endJob,
                Model.ModelJob modelJob,
                NaviController naviJobStarter,
                AniController aniJobstarter)
            : base(modelJob, endJob)
        {
            this.modelJob = modelJob;
            this.naviJobStarter = naviJobStarter;
            this.aniJobstarter = aniJobstarter;
        }

        public override void StartJob()
        {
            var firstAP = modelJob.aph.GetNowActionPoint();
            AddJob(CreateJobs(firstAP, modelJob.aph));
            base.StartJob();
        }

        public Queue<Job> CreateJobs(ActionPoint ap, ActionPointHandler aph)
        {
            var queue = new Queue<Job>();
            for (jobState i = jobState.navi; i < jobState.non; i++)
            {
                var job = new ModelHandlerJob(this, ap, aph.walkingState);
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

        void ReadNextAP()
        {
            var ap = modelJob.aph.GetNextActionPoint();
            if (ap != null)
            {
                AddJob(CreateJobs(ap, modelJob.aph));
                StartJob();
            }
            else
            {
                EndJob();
            }
        }
    }
}