using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NaviController))]
[RequireComponent(typeof(AniController))]
public class ModelPhysicsController : MonoBehaviour, IObjDetectorConnector_OnDetected
{
    public Model model;
    ActionPointHandler actionPointHandler;
    public NaviController naviController { private set; get; }
    public AniController aniController { private set; get; }

    private void Awake()
    {
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();
    }

    public void SetNextTargetPosition(Vector3 WPosition)
    {
        naviController.SetNextPosition(WPosition);
    }

    public void ReadNextAction()
    {
        var ap = actionPointHandler.GetNowActionPoint();
        if (ap.HasNoAction)
        {
            SetNextTargetPosition(actionPointHandler.GetNextActionPoint().transform.position);
        }
        else
        {
            naviController.MakeCorrect(ap.transform.position, ap.transform.forward);
        }
    }

    public void OnDetected(ObjDetector detector, Collider collider)
    {
        throw new System.NotImplementedException();
    }
}
