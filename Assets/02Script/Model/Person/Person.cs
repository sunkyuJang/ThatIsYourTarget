using System;
using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;


public class Person : Model
{
    public enum StateKinds { Normal, Notice, Warn, Follow, Wait, Attack, Avoid, Dead, Non }
    enum StateByDist { Notice = 3, Attack = 1 }
    [SerializeField]
    private Renderer modelRenderer;
    public Material belongTo
    {
        set { modelRenderer.material = value; }
        get { return modelRenderer.material; }
    }
    private List<TrackingPlayerState> trackingPlayerList { set; get; } = new List<TrackingPlayerState>();
    public override void ChangedState(int state)
    {
        switch ((StateKinds)state)
        {
            case StateKinds.Normal: break;
            case StateKinds.Notice: StartTrackingToPlayers(); break;
        }
    }

    private Coroutine procDoTrackingPlayers = null;
    public override void GetHit()
    {

    }

    public override void Removed(Collider collider)
    {
        switch (collider.tag)
        {
            case Player.playerTag:
                var player = GetComponent<Player>();
                RemoveFromTrackingPlayerState(player);
                break;
        }
    }

    public override void Contecting(Collider collider)
    {
        switch (collider.tag)
        {
            case Player.playerTag:
                var player = GetComponent<Player>();
                AddToTrackingPlayerState(player);

                // if (!belongTo.Equals(player.belongTo))
                // {
                //     var stateByDist = GetStateByDist(player.transform.position);

                //     ChangedState((int)stateByDist);
                // }
                break;
        }
    }

    void AddToTrackingPlayerState(Player player)
    {
        TrackingPlayerState trackingPlayerState = null;
        if (trackingPlayerList.Count == 0)
        {
            trackingPlayerState = new TrackingPlayerState(player);
        }
        else
        {
            var find = trackingPlayerList.Find(x => x.player == player);
            if (find.player == null)
            {
                trackingPlayerState = new TrackingPlayerState(player);
            }
        }

        if (trackingPlayerState != null)
        {
            trackingPlayerList.Add(trackingPlayerState);
            //하는 중...
            // 스테이트 체인지를 어디서?
        }
    }

    void RemoveFromTrackingPlayerState(Player player)
    {
        //it will just reservate for remove.
        try
        {
            var find = trackingPlayerList.Find(x => x.player == player);
            find.shouldRemove = true;
        }
        catch
        {
            print("something's wrong in trackingPlayerList");
        }
    }

    void StartTrackingToPlayers()
    {
        if (procDoTrackingPlayers == null)
            procDoTrackingPlayers = StartCoroutine(DoTrackingPlayers());
    }

    IEnumerator DoTrackingPlayers()
    {
        while (trackingPlayerList.Count != 0)
        {
            var data = trackingPlayerList[0];
            if (data.shouldRemove)
            {
                trackingPlayerList.Remove(data);
            }
            else
            {
                if (!data.isFollowing)
                {
                    yield return StartCoroutine(DoTrackingPlayer(data));
                }
            }

            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    IEnumerator DoTrackingPlayer(TrackingPlayerState data)
    {
        data.isFollowing = true;
        while (!data.shouldRemove)
        {

        }
        yield return null;
    }

    StateKinds GetStateByDist(Vector3 WPosition)
    {
        var dist = Vector3.Distance(modelPhysicsController.transform.position, WPosition);
        return dist == (float)StateByDist.Attack ? StateKinds.Attack : StateKinds.Notice;
    }

    class TrackingPlayerState
    {
        public Player player = null;
        public bool isFollowing = false;
        public bool shouldRemove = false;

        public TrackingPlayerState(Player player)
        {
            this.player = player;
        }
    }
    // public PersonModel model { private set; get; }

    // ActionPointHandler originalAPH;
    // [HideInInspector]
    // public ActionPointHandler actionPointHandler;
    // Transform NextPosition { set; get; } = null;

    // public enum AliveState { Alive, Stun, Dead }
    // public AliveState NowAliveState { protected set; get; } = AliveState.Alive;

    // public enum AlertLevel { Normal, Notice, Warn, Follow, Wait, Attack, Avoid, Non }
    // public AlertLevel BeforeAlertLevel { private set; get; } = AlertLevel.Non;
    // Coroutine nowPlayingAPH;
    // void StopNowPlayingAPH() { if (nowPlayingAPH != null) StopCoroutine(nowPlayingAPH); nowPlayingAPH = null; }
    // Coroutine DoStateProcess;

    // [HideInInspector]
    // //public ConversationEntry conversationEntry = null;
    // public bool IsStandingOnPosition(Vector3 targetWorldPosition)
    // {
    //     return Vector3.Distance(model.transform.position, targetWorldPosition) <= 0.25f;
    // }
    // private void Awake()
    // {
    //     model = transform.Find("Model").GetComponent<PersonModel>();
    //     model.Person = this;

    //     originalAPH = transform.Find("ActionPointHandler").GetComponent<ActionPointHandler>();
    //     actionPointHandler = originalAPH;
    // }

    // private void Start()
    // {
    //     StartAPH();
    // }

    // void StartAPH()
    // {
    //     if (nowPlayingAPH != null)
    //     {
    //         StopNowPlayingAPH();
    //     }

    //     //nowPlayingAPH = StartCoroutine(DoAction());
    // }

    // IEnumerator DoAction()
    // {
    //     while (true)
    //     {
    //         var nextActionPoint = actionPointHandler.GetNextActionPoint() as PersonActionPoint;

    //         if (!IsStandingOnPosition(nextActionPoint.transform.position))
    //         {
    //             yield return StartCoroutine(DoMove(nextActionPoint));
    //         }

    //         if (nextActionPoint.state != (int)PersonActionPoint.StateKind.non)
    //         {
    //             model.MakeLookAt(nextActionPoint.transform.forward);

    //             switch (nextActionPoint.state)
    //             {
    //                 case (int)PersonActionPoint.StateKind.sitting: model.SetSittingAnimation(nextActionPoint.SittingNum); break;
    //                 case (int)PersonActionPoint.StateKind.lookAround: model.SetLookAroundAnimation(); break;
    //                 case (int)PersonActionPoint.StateKind.standing: model.SetIdleAnimation(); break;
    //             }

    //             //nextActionPoint.StartTimeCount();
    //             //yield return new WaitUntil(() => !nextActionPoint.IsDoing);
    //         }

    //         yield return new WaitForFixedUpdate();
    //     }
    // }

    // IEnumerator DoMove(ActionPoint nextActionPoint)
    // {
    //     model.SetWalkState(BeforeAlertLevel == AlertLevel.Normal ? 1 : 2);
    //     yield return new WaitUntil(() => model.NavMeshAgent.enabled);
    //     model.SetNextPosition(nextActionPoint.transform.position);
    //     yield return new WaitUntil(() => IsStandingOnPosition(nextActionPoint.transform.position));
    //     model.SetPositionCorrectly(nextActionPoint.transform.position);
    // }

    // public void GetHit()
    // {
    //     model.GetHit();
    // }

    // public void OnContecting(ObjDetector detector, Collider collider)
    // {
    //     //contecting Person
    //     var alertLevel = GetAlertLevel(detector.transform.position, collider.transform.position);
    //     alertLevel = AlertLevel.Notice;
    //     ChangeAlertState(alertLevel, collider.transform.position);
    // }

    // AlertLevel GetAlertLevel(Vector3 centerPosition, Vector3 personPosition)
    // {
    //     var nowDist = Vector3.Distance(centerPosition, personPosition);
    //     return nowDist > 3f ? AlertLevel.Notice : AlertLevel.Attack;
    // }

    // public void ChangeAlertState(AlertLevel level, Vector3 targetPosition)
    // {
    //     if (BeforeAlertLevel == level)
    //     {
    //         //Action<Vector3> action;
    //         switch (BeforeAlertLevel)
    //         {
    //             case AlertLevel.Notice: //is Notice APPosition Changed? than Chnaged.
    //                 StopNowPlayingAPH();
    //                 actionPointHandler.ChangeAPPositionAndLookAt(actionPointHandler.actionPoints.Count - 1, model.transform.position, targetPosition);
    //                 StartAPH();
    //                 break;
    //         }
    //     }
    //     else
    //     {
    //         BeforeAlertLevel = level;

    //         if (DoStateProcess != null)
    //             StopCoroutine(DoStateProcess);

    //         if (BeforeAlertLevel == AlertLevel.Notice
    //             || BeforeAlertLevel == AlertLevel.Warn
    //             || BeforeAlertLevel == AlertLevel.Attack)
    //         {
    //             if (actionPointHandler.comingFromOther != null)
    //                 actionPointHandler.comingFromOther(actionPointHandler);

    //             IntoNoticeState(targetPosition);
    //         }
    //         else
    //         {
    //             IntoNormalState();
    //         }
    //     }
    // }

    // bool IsAPHPerentOfThisPerson()
    // {
    //     return originalAPH.Equals(actionPointHandler);
    // }

    // void TimeOutForFinding()
    // {
    //     ChangeAlertState(AlertLevel.Normal, Vector3.zero);
    // }

    // void IntoNoticeState(Vector3 targetPosition)
    // {
    //     model.SetAlertLevel(AlertLevel.Notice);

    //     //var aph = APHManager.Instance.GetAPHForNotice(targetPosition, model.transform.position);
    //     //ChangeAPHandler(aph);
    // }

    // void IntoNormalState()
    // {
    //     ChangeAPHandler(null);
    //     model.SetAlertLevel(AlertLevel.Normal);
    // }

    // public void SetBelongTo(Material material)
    // {
    //     model.SetBelong(material);
    // }

    // public void ChangeAPHandler(ActionPointHandler APHandler)
    // {
    //     if (nowPlayingAPH != null)
    //     {
    //         StopNowPlayingAPH();
    //         actionPointHandler.comingFromOther?.Invoke(actionPointHandler);
    //     }

    //     if (APHandler == null)
    //         actionPointHandler = originalAPH;
    //     else
    //         actionPointHandler = APHandler;

    //     StartAPH();
    // }
}
