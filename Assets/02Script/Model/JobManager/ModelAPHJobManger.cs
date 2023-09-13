using System;
using UnityEngine;

public class ModelAPHJobManger : JobManager
{
    enum State { CreatingAnimationPlayerJob, End, Non }
    IJobStarter<ModelJob> TargetJobStarter { get; set; }
    AnimationPointHandler OriginalAPH { get; set; }
    AnimationPointHandler TargetAPH { get; set; }
    Action ReservatedAction { get; set; }
    JobManager JobManager { get; set; }

    public ModelAPHJobManger(
                object section,
                Action endjob,
                Transform originalAPHGroup,
                IJobStarter<ModelJob> targetJobStarter) : base(section, endjob)
    {
        OriginalAPH = originalAPHGroup.Find("OriginalAPH").GetComponent<AnimationPointHandler>();
        TargetJobStarter = targetJobStarter;
        SetAPH();
    }

    public void SetAPH(AnimationPointHandler targetAPH = null, Action ReservatedAction = null)
    {
        TargetAPH = targetAPH == null ? OriginalAPH : targetAPH;
        this.ReservatedAction = ReservatedAction;
    }

    public override void StartJob()
    {
        if (JobManager == null) JobManager = new JobManager(EndJobManager);
        var job = new ModelJob(JobManager, TargetAPH, ReturnAPH);
        job.jobAction = () => TargetJobStarter.StartJob(job);
        JobManager.AddJob(job);
        JobManager.StartJob();
    }

    void EndJobManager()
    {
        if (ReservatedAction == null)
        {
            SetAPH();
        }
        else
        {
            ReservatedAction.Invoke();
        }
    }
    public void ReturnAPH(AnimationPointHandler APH)
    {
        if (APH != OriginalAPH)
            APHManager.Instance.ReturnAPH(APH);
    }


    public class ModelJob : Job
    {
        public AnimationPointHandler aph { private set; get; }
        public Action<AnimationPointHandler> returnAPH { private set; get; }
        public ModelJob(JobManager jobManager, AnimationPointHandler aph, Action<AnimationPointHandler> returnAPH) : base(jobManager)
        {
            this.aph = aph;
            this.returnAPH = returnAPH;
        }
    }
}
