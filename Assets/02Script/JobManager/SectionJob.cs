using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SectionJob : Job
{
    protected Job highLevelJob { set; get; }
    protected ISectionJobChecker sectionJobChecker { set; get; }
    public SectionJob(Job highLevelJob, IJobStarter jobStarter, ISectionJobChecker sectionJobChecker, Action endAction, Action exceptionAction)
        : base(jobStarter, endAction, exceptionAction)
    {
        this.highLevelJob = highLevelJob;
        this.sectionJobChecker = sectionJobChecker;
    }
    public override void EndJob()
    {
        if (sectionJobChecker.IsSameSection(highLevelJob))
        {
            base.EndJob();
        }
        else
        {
            exceptionAction();
        }
    }
}
