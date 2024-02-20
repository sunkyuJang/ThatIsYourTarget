using System;
using System.Collections.Generic;
using JExtentioner;

public class ModelAnimationPlayerJobManager : JobManager
{
    public enum JobState { aniTrackingUpdate, navi, ani, done, non }
    private ModelAPHJobManger.ModelJob modelJob;
    private IJobStarter<ModelHandlerJob> naviJobStarter;
    private IJobStarter<ModelHandlerJob> aniJobstarter;

    public ModelAnimationPlayerJobManager(
            Action runAfterJobEnd,
            ModelAPHJobManger.ModelJob modelJob,
            NaviController naviJobStarter,
            AniController aniJobstarter)
        : base(modelJob, runAfterJobEnd)
    {
        this.modelJob = modelJob;
        this.naviJobStarter = naviJobStarter;
        this.aniJobstarter = aniJobstarter;
    }

    public override void StartJob()
    {
        var firstAP = modelJob.aph.GetNowAnimationPoint();
        AddJob(CreateJobs(firstAP, modelJob.aph));
        base.StartJob();
    }

    public Queue<Job> CreateJobs(AnimationPoint ap, AnimationPointHandler aph)
    {
        var queue = new Queue<Job>();
        for (int i = 0; i < EnumExtentioner.GetEnumSize<JobState>() - 1; i++)
        {
            var job = new ModelHandlerJob(this, ap, aph.walkingState);
            Action action = null;
            switch ((JobState)i)
            {
                case JobState.aniTrackingUpdate:
                    action = () =>
                    {
                        // for updating tracking
                        job.jobState = JobState.aniTrackingUpdate;
                        aniJobstarter.StartJob(job);
                    };
                    break;
                case JobState.navi:
                    action = () =>
                    {
                        naviJobStarter.StartJob(job);
                    };
                    break;
                case JobState.ani:
                    action = () =>
                    {
                        // for updating animation
                        job.jobState = JobState.ani;
                        aniJobstarter.StartJob(job);
                    };
                    break;
                case JobState.done:
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
        public JobState jobState = JobState.aniTrackingUpdate;
        public ModelHandlerJob(JobManager jobManager, AnimationPoint ap, AnimationPointHandler.WalkingState walkingState) : base(jobManager)
        {
            this.ap = ap;
            this.walkingState = walkingState;
        }
    }
}


