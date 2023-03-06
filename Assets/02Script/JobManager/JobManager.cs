using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager
{
    protected Queue<Action> jobList = new Queue<Action>();
    Action doEndJob;
    bool canJobRunning = true;

    public JobManager(Queue<Action> jobList, Action doEndJob)
    {
        this.jobList = jobList;
        this.doEndJob = doEndJob;
    }
    public void StopRunning() { canJobRunning = false; }
    public virtual void StartJob()
    {
        if (canJobRunning && jobList.Count > 0)
        {
            jobList.Dequeue().Invoke();
        }
        else
        {
            EndJob();
        }
    }

    public virtual void EndJob()
    {
        doEndJob.Invoke();
    }
}
