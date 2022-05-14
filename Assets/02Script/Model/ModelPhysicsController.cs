using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModelPhysicsController : MonoBehaviour, IObjDetectorConnector_OnContecting, IObjDetectorConnector_OnRemoved
{
    Model model;
    ActionPointHandler actionPointHandler { set; get; }
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

    public void SetNextTargetPosition(Vector3 WPosition)
    {
        naviController.SetNextPosition(WPosition);
    }

    public void ReadNowAction()
    {
        var ap = actionPointHandler.GetNowActionPoint();
        if (ap.HasAction)
        {
            naviController.MakeCorrect(ap.transform.position, ap.transform.forward);
            aniController.StartAni(ap);
        }
        else
        {
            ReadNextAction();
        }
    }

    public void ReadNextAction()
    {
        SetNextTargetPosition(actionPointHandler.GetNextActionPoint().transform.position);
    }

    public void OnContecting(ObjDetector detector, Collider collider)
    {
        model.Contecting(collider);
    }

    public void OnRemoved(ObjDetector detector, Collider collider)
    {
        model.Removed(collider);
    }
}