using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModelHandler : MonoBehaviour, IObjDetectorConnector_OnDetected, IObjDetectorConnector_OnRemoved
{
    Model model;
    public ActionPointHandler actionPointHandler { private set; get; }
    public NavController naviController { private set; get; }
    public AniController aniController { private set; get; }
    RagDollHandler ragDollHandler { set; get; }
    private void Awake()
    {
        model = GetComponentInParent<Model>();
        naviController = GetComponent<NavController>();
        aniController = GetComponent<AniController>();
        ragDollHandler = GetComponent<RagDollHandler>();
    }
    private void Update()
    {
        if (true)
        {
            var sd = 10;
        }
    }
    public void SetAPH(ActionPointHandler handler)
    {
        if (actionPointHandler != null)
            model.ReturnAPH(actionPointHandler);

        actionPointHandler = handler;
        SetNextTargetPosition(handler.GetNowActionPoint());
    }

    public void ChageLastAPPosition(Transform target)
    {
        var lastAP = actionPointHandler.GetActionPoint(actionPointHandler.GetActionCount - 1);
        lastAP.SetPositionForTracking(transform, target, true);
    }

    public void SetNextTargetPosition(ActionPoint ap)
    {
        naviController.SetNextPosition(ap);
    }

    public void ReadNowAction()
    {
        StartCoroutine(DoReadNowAction());
    }

    IEnumerator DoReadNowAction()
    {
        var ap = actionPointHandler.GetNowActionPoint();

        if (ap.HasAction)
        {
            aniController.DoAction(ap);
        }
        else
        {
            ReadNextAction();
        }
        yield return null;
    }

    public void ReadNextAction()
    {
        if (actionPointHandler.IsReachedToEnd)
        {
            model.SetOriginalAPH();
        }
        else
        {
            SetNextTargetPosition(actionPointHandler.GetNextActionPoint());
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