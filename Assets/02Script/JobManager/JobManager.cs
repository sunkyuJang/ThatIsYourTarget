using System;
using System.Collections.Generic;
using UnityEngine;
public class JobManager
{
    private Action RunAfterJobEnd;
    private Queue<Job> jobList = new Queue<Job>();
    private object section = null;
    public bool IsSameSection(object section) => this.section.Equals(section);
    public JobManager(object section, Action runAfterJobEnd)
    {
        this.section = section;
        this.RunAfterJobEnd = runAfterJobEnd;
    }
    public JobManager(Action runAfterJobEnd)
    {
        this.RunAfterJobEnd = runAfterJobEnd;
    }

    public virtual void StartJob()
    {
        NextJob();
    }
    public virtual void NextJob()
    {
        if (jobList.Count <= 0)
        {
            EndJob();
        }
        else
        {
            jobList.Dequeue().ProcesseJob();
        }
    }
    public virtual void EndJob()
    {
        RunAfterJobEnd.Invoke();
    }
    public virtual void CancleJob()
    {
        jobList.Clear();
    }
    public virtual void AddJob(Job job)
    {
        jobList.Enqueue(job);
    }
    public virtual void AddJob(Queue<Job> job)
    {
        jobList = job;
    }
}

public class Job
{
    private JobManager jobManager;
    public Action jobAction;
    public Job(JobManager jobManager)
    {
        this.jobManager = jobManager;
    }

    public virtual void ProcesseJob()
    {
        if (jobAction == null) Debug.Log("jobAction is missing");
        jobAction?.Invoke();
    }

    // each job must call endJob Function when job is done.
    public virtual void EndJob()
    {
        jobManager.NextJob();
    }
}



