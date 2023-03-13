using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager
{
    protected Queue<Action> jobList = new Queue<Action>();
    Action doEndJob;
    bool canRunningJob = true;

    public JobManager(Queue<Action> jobList, Action doEndJob)
    {
        this.jobList = jobList;
        this.doEndJob = doEndJob;
    }
    public void StopRunning() { canRunningJob = false; }
    public virtual void StartJob()
    {
        if (canRunningJob)
        {
            if (jobList.Count > 0)
            {
                jobList.Dequeue().Invoke();
            }
            else
            {
                EndJob();
            }
        }
        else
        {
            // Destory job
        }
    }

    public virtual void EndJob()
    {
        doEndJob.Invoke();
    }
}
