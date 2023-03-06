using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Job
{
    protected IJobStarter jobStarter;
    protected Action endAction;
    protected Action exceptionAction;
    protected Job section;
    public Job(IJobStarter jobStarter, Action endAction, Action exceptionAction)
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
            endAction.Invoke();
        else if (exceptionAction != null)
            exceptionAction.Invoke();
        else
        {
            Debug.Log("job has exception");
        }
    }

    public void ExceptionJob()
    {

    }
}
