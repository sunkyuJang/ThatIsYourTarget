using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;


public class Person : MonoBehaviour, IObjDetectorConnector_OnContecting
{
    public PersonModel model { private set; get; }

    [HideInInspector]
    public ActionPointHandler actionPointHandler;
    Transform NextPosition { set; get; } = null;

    public enum AliveState { Alive, Stun, Dead }
    public AliveState NowAliveState { protected set; get; } = AliveState.Alive;

    public enum AlertLevel { Normal, Notice, Attack, Avoid, Non }
    public AlertLevel NowAlertLevel { protected set; get; } = AlertLevel.Normal;
    AlertLevel BeforeAlertLevel { set; get; } = AlertLevel.Non;
    Coroutine nowPlayingAPs;

    public bool IsStandingOnPosition(Vector3 targetWorldPosition)
    {
        return Vector3.Distance(model.transform.position, targetWorldPosition) <= 0.25f;
    }
    private void Awake()
    {
        model = transform.Find("Model").GetComponent<PersonModel>();

        actionPointHandler = transform.Find("ActionPointHandler").GetComponent<ActionPointHandler>();
    }

    private void Start()
    {
        StartAPs();
    }

    void StartAPs()
    {
        nowPlayingAPs = StartCoroutine(DoAction());
    }

    IEnumerator DoAction()
    {
        while (true)
        {
            var nextActionPoint = actionPointHandler.GetNextActionPoint();

            if (!IsStandingOnPosition(nextActionPoint.transform.position))
            {
                model.SetWalkState(NowAlertLevel == AlertLevel.Normal ? 1 : 2);
                yield return new WaitUntil(() => model.NavMeshAgent.enabled);
                model.SetNextPosition(nextActionPoint.transform.position);
                yield return new WaitUntil(() => IsStandingOnPosition(nextActionPoint.transform.position));
                model.SetPositionCorrectly(nextActionPoint.transform.position);
            }

            if (nextActionPoint.state != ActionPoint.StateKind.non)
            {
                model.MakeLookAt(nextActionPoint.transform.forward);

                switch (nextActionPoint.state)
                {
                    case ActionPoint.StateKind.sitting: model.SetSittingAnimation(nextActionPoint.SittingNum); break;
                    case ActionPoint.StateKind.lookAround: model.SetLookAroundAnimation(); break;
                    case ActionPoint.StateKind.idle: model.SetIdleAnimation(); break;
                }

                nextActionPoint.StartTimeCount();
                yield return new WaitUntil(() => !nextActionPoint.IsDoing);
            }

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
        ChangeAlertState(nowDist > 5f ? AlertLevel.Notice : AlertLevel.Attack);
    }

    void ChangeAlertState(AlertLevel level)
    {
        if (NowAlertLevel != BeforeAlertLevel)
        {
            BeforeAlertLevel = NowAlertLevel;

            if (BeforeAlertLevel == AlertLevel.Notice || BeforeAlertLevel == AlertLevel.Attack)
            {
                var timeData = TimeCounter.Instance.SetTimeCounting(5f, 1f, TimeOutForFinding);

            }
            model.SetAlertLevel(AlertLevel.Notice);
        }
    }


    void TimeOutForFinding()
    {

    }

    public void SetBelongTo(Material material)
    {
        model.SetBelong(material);
    }

    public void ChangeAPHandler(ActionPointHandler APHandler)
    {
        StopCoroutine(nowPlayingAPs);
        if (APHandler == null)
            actionPointHandler = transform.Find("ActionPointHandler").GetComponent<ActionPointHandler>();
        else
            actionPointHandler = APHandler;

        StartAPs();
    }
}
