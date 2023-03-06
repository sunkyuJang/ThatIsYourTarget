using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Job
{
    protected IJobStarter jobStarter;
    protected Action<Job> endAction;
    protected Action<Job> exceptionAction;
    protected Job section;
    public Job(IJobStarter jobStarter, Action<Job> endAction, Action<Job> exceptionAction)
    {
        this.jobStarter = jobStarter;
        this.endAction = endAction;
        this.exceptionAction = exceptionAction;
    }
    public virtual void StartJob()
    {
        jobStarter.StartJob(this);
    }
    public virtual void EndJob()
    {
        if (endAction != null)
            endAction.Invoke(this);
        else if (exceptionAction != null)
            exceptionAction.Invoke(this);
        else
        {
            Debug.Log("job has exception");
        }
    }

    public void ExceptionJob()
    {

    }
}
