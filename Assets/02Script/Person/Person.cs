using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;


public class Person : MonoBehaviour, IObjDetectorConnector_OnContecting
{
    public PersonModel model { private set; get; }

    ActionPointHandler actionPointHandler;
    Transform NextPosition { set; get; } = null;

    public enum AliveState { Alive, Stun, Dead }
    public AliveState NowAliveState { protected set; get; } = AliveState.Alive;

    public enum AlertLevel { Normal, Notice, Attack, Avoid }
    public AlertLevel NowAlertLevel { protected set; get; } = AlertLevel.Normal;
    Coroutine nowPlayingAPs;
    public bool IsStandingOnPosition { protected set; get; }
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
        model.SetToWalkAnimation();
        while (true)
        {
            IsStandingOnPosition = false;
            var nextActionPoint = actionPointHandler.GetNextActionPoint();
            model.SetNextPosition(nextActionPoint.transform.position);
            yield return new WaitUntil(() => Vector3.Distance(model.transform.position, nextActionPoint.transform.position) <= 0.5f);
            IsStandingOnPosition = true;

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
                model.SetToWalkAnimation();
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
