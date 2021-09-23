using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;


public class Person : MonoBehaviour, IObjDetectorConnector_OnContecting
{
    PersonModel model;

    ActionPointHandler actionPointHandler;
    Transform NextPosition { set; get; } = null;

    public enum AliveState { Alive, Stun, Dead }
    public AliveState NowAliveState { protected set; get; } = AliveState.Alive;

    public enum AlertLevel { Normal, Notice, Attack, Avoid }
    public AlertLevel NowAlertLevel { protected set; get; } = AlertLevel.Normal;
    private void Awake()
    {
        model = transform.Find("Model").GetComponent<PersonModel>();

        actionPointHandler = transform.Find("ActionPointHandler").GetComponent<ActionPointHandler>();
    }

    private void Start()
    {
        StartCoroutine(DoAction());
    }

    IEnumerator DoAction()
    {
        var actionIndex = 0;
        while (true)
        {
            var nextActionPoint = actionPointHandler.GetActionPoint(actionIndex++);
            model.SetNextPosition(nextActionPoint.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(model.transform.position, nextActionPoint.transform.position) <= 0.5f);

            if (nextActionPoint.state != actionPoint.StateKind.non)
            {
                model.MakeLookAt(nextActionPoint.transform.forward);

                switch (nextActionPoint.state)
                {
                    case actionPoint.StateKind.sitting: model.SetSittingAnimation(1); break;
                }

                nextActionPoint.StartTimeCount();
                yield return new WaitUntil(() => !nextActionPoint.IsDoing);
                model.SetToIdleAnimation();
            }

            actionIndex %= actionPointHandler.GetActionCount;

            yield return new WaitForFixedUpdate();
        }
    }
    public void GetHit()
    {
        model.GetHit();
    }

    public void OnContecting(ObjDetector detector, Collider collider)
    {
        var nowDist = Vector3.Distance(detector.transform.position, collider.transform.position);
        if (nowDist < 3f)
        {
            if (NowAlertLevel != AlertLevel.Attack)
                NowAlertLevel = AlertLevel.Attack;
        }
    }

    protected void MakeAlertLevel(AlertLevel alertLevel)
    {
        switch (alertLevel)
        {
            case AlertLevel.Attack: break;
        }
    }

    public void SetBelongTo(Material material)
    {
        model.SetBelong(material);
    }
}
