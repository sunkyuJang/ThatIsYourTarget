using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHandlerJobManager : JobManager
{
    ISectionJobChecker sectionJobChecker;
    Job parentJob;
    IJobStarter naviJobStarter;
    IJobStarter aniJobStarter;
    ActionPointHandler aph;
    enum JobKind { naviJob, aniJob, nextJob, SetNextJob, doneJob, non }
    public ModelHandlerJobManager(Action doEndJob, IJobStarter naviJobStarter, IJobStarter aniJobStarter, Job parentJob, ActionPointHandler aph, ISectionJobChecker sectionJobChecker)
        : base(null, doEndJob)
    {
        jobList = CreatingJob(parentJob);
        this.sectionJobChecker = sectionJobChecker;
        this.parentJob = parentJob;
        this.naviJobStarter = naviJobStarter;
        this.aniJobStarter = aniJobStarter;
        this.aph = aph;
    }

    Queue<Action> CreatingJob(Job parentJob)
    {
        var jobList = new Queue<Action>();
        var nowAP = aph.GetNowActionPoint();
        var isAPHDone = aph.isAPHDone;
        var state = isAPHDone ? (int)JobKind.doneJob : 0;
        for (int i = state; i < (int)JobKind.non; i++)
        {
            Action action = null;
            switch ((JobKind)i)
            {
                case JobKind.naviJob:
                    {
                        action = (new ModelHandler.ModelHandlerJob(sectionJobChecker, parentJob, naviJobStarter, nowAP, StartJob, null)).StartJob;
                    }
                    break;
                case JobKind.aniJob:
                    {
                        if (nowAP.HasAction)
                            action = (new ModelHandler.ModelHandlerJob(sectionJobChecker, parentJob, aniJobStarter, nowAP, StartJob, null)).StartJob;
                        else continue;
                    }
                    break;
                case JobKind.nextJob:
                    {
                        action = ReadNextAP;
                    }
                    break;
                case JobKind.doneJob:
                    if (isAPHDone)
                        action = EndJob;
                    break;
            }

            if (action != null)
                jobList.Enqueue(action);
        }
        return jobList;
    }

    void ReadNextAP()
    {
        aph.GetNextActionPoint();
        CreatingJob(parentJob);
    }

    public ModelHandler.ModelHandlerJob GetNewModelHandlerJob(Job job, IJobStarter jobStarter, Action endAction, Action exceptionAction)
    {
        var ap = aph.GetNowActionPoint();
        var sectionChecker = this as ISectionJobChecker;
        return new ModelHandler.ModelHandlerJob(sectionChecker, job, jobStarter, ap, endAction, exceptionAction);
    }
}
