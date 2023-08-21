using System;
using System.Collections.Generic;

public class ModelHandlerJobManager : JobManager
{
    enum jobState { navi, ani, done, non }
    private Model.ModelJob modelJob;
    private IJobStarter<ModelHandlerJob> naviJobStarter;
    private IJobStarter<ModelHandlerJob> aniJobstarter;

    public ModelHandlerJobManager(
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

    public Queue<Job> CreateJobs(AnimationPoint ap, AnimationPointHandler aph)
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
    public class ModelHandlerJob : Job
    {
        public AnimationPoint ap { private set; get; }
        public AnimationPointHandler.WalkingState walkingState { private set; get; }
        public ModelHandlerJob(JobManager jobManager, AnimationPoint ap, AnimationPointHandler.WalkingState walkingState) : base(jobManager)
        {
            this.ap = ap;
            this.walkingState = walkingState;
        }
    }
}


