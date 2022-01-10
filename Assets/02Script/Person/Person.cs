using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;


public class Person : MonoBehaviour, IObjDetectorConnector_OnContecting
{
    public PersonModel model { private set; get; }

    ActionPointHandler originalAPH;
    [HideInInspector]
    public ActionPointHandler actionPointHandler;
    Transform NextPosition { set; get; } = null;

    public enum AliveState { Alive, Stun, Dead }
    public AliveState NowAliveState { protected set; get; } = AliveState.Alive;

    public enum AlertLevel { Normal, Notice, Warn, Wait, Attack, Avoid, Non }
    public AlertLevel BeforeAlertLevel { private set; get; } = AlertLevel.Non;
    Coroutine nowPlayingAPs;
    Coroutine DoStateProcess;

    public ConversationEntry conversationEntry = null;
    public bool IsStandingOnPosition(Vector3 targetWorldPosition)
    {
        return Vector3.Distance(model.transform.position, targetWorldPosition) <= 0.25f;
    }
    private void Awake()
    {
        model = transform.Find("Model").GetComponent<PersonModel>();
        model.Person = this;

        originalAPH = transform.Find("ActionPointHandler").GetComponent<ActionPointHandler>();
        actionPointHandler = originalAPH;
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
                model.SetWalkState(BeforeAlertLevel == AlertLevel.Normal ? 1 : 2);
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
        //contecting Person
        var nowDist = Vector3.Distance(detector.transform.position, collider.transform.position);
        ChangeAlertState(AlertLevel.Notice, collider.transform.position);
    }

    public void ChangeAlertState(AlertLevel level, Vector3 targetPosition)
    {
        if (BeforeAlertLevel != level)
        {
            BeforeAlertLevel = level;

            if (DoStateProcess != null)
                StopCoroutine(DoStateProcess);

            if (BeforeAlertLevel == AlertLevel.Notice
                || BeforeAlertLevel == AlertLevel.Warn
                || BeforeAlertLevel == AlertLevel.Attack)
            {
                if (actionPointHandler.comingFromAPH != null)
                    actionPointHandler.comingFromAPH(actionPointHandler);


                StartCoroutine(DoNoticeState(targetPosition));
                model.SetAlertLevel(AlertLevel.Notice);
            }
            else
            {
                ChangeAPHandler(null);
                model.SetAlertLevel(AlertLevel.Normal);
            }
        }
    }

    bool IsAPHPerentOfThisPerson()
    {
        return originalAPH.Equals(actionPointHandler);
    }

    void TimeOutForFinding()
    {
        ChangeAlertState(AlertLevel.Normal, Vector3.zero);
    }

    IEnumerator DoNoticeState(Vector3 targetPosition)
    {
        var aph = APHManager.Instance.GetAPHForNotice(targetPosition, model.transform.position);
        ChangeAPHandler(aph);
        print(true);
        yield return new WaitUntil(() => model.animator.GetCurrentAnimatorStateInfo(0).IsName("Villager@Idle01"));

        //var timeData = TimeCounter.Instance.SetTimeCounting(5f, 1f, TimeOutForFinding);
        yield return new WaitUntil(() => !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Villager@Idle01"));
        print(false);

        ChangeAlertState(AlertLevel.Normal, Vector3.zero);
        yield return null;
    }

    public void SetBelongTo(Material material)
    {
        model.SetBelong(material);
    }

    public void ChangeAPHandler(ActionPointHandler APHandler)
    {
        if (nowPlayingAPs != null)
            StopCoroutine(nowPlayingAPs);

        if (APHandler == null)
            actionPointHandler = originalAPH;
        else
            actionPointHandler = APHandler;

        StartAPs();
    }
}
