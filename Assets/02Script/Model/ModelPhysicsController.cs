using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModelPhysicsController : MonoBehaviour, IObjDetectorConnector_OnDetected, IObjDetectorConnector_OnRemoved
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

    public void SetAPH(ActionPointHandler handler)
    {
        if (actionPointHandler != null)
            APHManager.Instance.ReturnAPH(actionPointHandler);

        actionPointHandler = handler;
        SetNextTargetPosition(handler.GetNowActionPoint().transform.position);
    }

    public void ChageLastAPPosition(Transform target)
    {
        var lastAP = actionPointHandler.GetActionPoint(actionPointHandler.GetActionCount - 1);
        lastAP.SetPositionForTracking(transform, target, false);
        SetNextTargetPosition(lastAP.transform.position);
    }

    public void SetNextTargetPosition(Vector3 WPosition)
    {
        naviController.SetNextPosition(WPosition);
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
            naviController.MakeCorrect(ap.transform.position, ap.transform.forward);
            yield return new WaitUntil(() => naviController.IsPositionAndRotationGetCorrect);
            aniController.StartAni(ap);
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
            SetNextTargetPosition(actionPointHandler.GetNextActionPoint().transform.position);
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