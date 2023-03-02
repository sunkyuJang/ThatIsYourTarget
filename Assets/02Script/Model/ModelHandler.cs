using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModelHandler : MonoBehaviour, IObjDetectorConnector_OnDetected, IObjDetectorConnector_OnRemoved
{
    Model model;
    public ActionPointHandler actionPointHandler { private set; get; }
    public IModelHandlerJobStarter naviJobStarter { private set; get; }
    public IModelHandlerJobStarter aniJobStarter { private set; get; }
    RagDollHandler ragDollHandler { set; get; }
    private Queue<Action> jobList = new Queue<Action>();
    private void Awake()
    {
        model = GetComponentInParent<Model>();
        naviJobStarter = GetComponent<NavController>() as IModelHandlerJobStarter;
        aniJobStarter = GetComponent<AniController>() as IModelHandlerJobStarter;
        ragDollHandler = GetComponent<RagDollHandler>();
    }
    public void SetJob(List<Action> jobs)
    {
        jobs.ForEach(x => jobList.Enqueue(x));
    }
    public void StartNextJob()
    {
        if (jobList.Count > 0)
        {
            jobList.Dequeue().Invoke();
        }
    }
    public void SetAPH(ActionPointHandler handler)
    {
        if (actionPointHandler != null)
            model.ReturnAPH(actionPointHandler);

        actionPointHandler = handler;
        jobList.Clear();

        SetJob(
            new List<Action>()
                {
                    SetNextTargetPosition,
                    ReadNowAction
                });

        StartNextJob();
    }
    public void ChageLastAPPosition(Transform target)
    {
        var lastAP = actionPointHandler.GetActionPoint(actionPointHandler.GetActionCount - 1);
        lastAP.SetPositionForTracking(transform, target, true);
    }

    public void SetNextTargetPosition()
    {
        naviJobStarter.StartJob(actionPointHandler.GetNowActionPoint());
    }

    private void ReadNowAction()
    {
        var ap = actionPointHandler.GetNowActionPoint();
        if (ap.HasAction)
        {
            aniJobStarter.StartJob(ap);
        }
        else
        {
            ReadNextAction();
        }
    }

    public void ReadNextAction()
    {
        if (actionPointHandler.IsReachedToEnd)
        {
            model.GetNextAPH();
        }
        else
        {
            SetNextTargetPosition();
        }
    }

    public void OnRemoved(ObjDetector detector, Collider collider)
    {
        model.Removed(collider);
    }

    public void OnDetected(ObjDetector detector, Collider collider)
    {
        model.Contected(collider);
    }
}