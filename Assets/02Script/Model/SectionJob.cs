using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SectionJob : Job
{
    protected Job highLevelJob { set; get; }
    protected ISectionJobChecker sectionJobChecker { set; get; }
    public SectionJob(Job highLevelJob, IJobStarter jobStarter, ISectionJobChecker sectionJobChecker, Action<Job> endAction, Action<Job> exceptionAction)
        : base(jobStarter, endAction, exceptionAction)
    {
        this.highLevelJob = highLevelJob;
        this.sectionJobChecker = sectionJobChecker;
    }
    public void StartJob(Job job)
    {
        if (job.Equals(highLevelJob))
            jobStarter.StartJob(this);
    }

    public override void EndJob()
    {
        if (sectionJobChecker.IsSameSection(highLevelJob))
        {
            base.EndJob();
        }
        else
        {
            exceptionAction(this);
        }
    }
}
