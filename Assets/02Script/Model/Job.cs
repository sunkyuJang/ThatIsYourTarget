using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Job
{
    protected IJobStarter jobStarter;
    protected Action endAction;
    protected Action exceptionAction;
    public void StartJob()
    {
        jobStarter.StartJob(this);
    }
    public void EndJob()
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
