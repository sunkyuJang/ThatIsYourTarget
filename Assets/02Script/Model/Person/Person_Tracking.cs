using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMath;


public partial class Person : Model
{
    private List<CheckingPlayerState> checkingPlayerStates { set; get; } = new List<CheckingPlayerState>();
    private Coroutine procDoCheckingPlayers = null;
    private Coroutine procDoTrackingClosePlayer = null;
    public override void Contected(Collider collider)
    {
        switch (collider.tag)
        {
            case Player.playerTag:
                var player = collider.GetComponent<Player>();
                if (player != null)
                    AddToTrackingPlayerState(player);
                break;
        }
    }
    public override void Removed(Collider collider)
    {
        switch (collider.tag)
        {
            case Player.playerTag:
                var player = collider.GetComponent<Player>();
                RemoveFromTrackingPlayerState(player);
                break;
        }
    }

    void AddToTrackingPlayerState(Player player)
    {
        CheckingPlayerState checkingPlayerState = null;
        if (checkingPlayerStates.Count == 0)
        {
            checkingPlayerState = new CheckingPlayerState(player);
        }
        else
        {
            var find = checkingPlayerStates.Find(x => x.player == player);
            if (find.player == null)
            {
                checkingPlayerState = new CheckingPlayerState(player);
            }
        }

        if (checkingPlayerState != null)
        {
            checkingPlayerStates.Add(checkingPlayerState);
            CheckPlayersState();
        }
    }

    void RemoveFromTrackingPlayerState(Player player)
    {
        //it will just reservate for remove.
        if (checkingPlayerStates.Count > 0)
        {
            var find = checkingPlayerStates.Find(x => x.player == player);
            if (find != null)
            {
                Debug.Log("getIn");
                find.shouldRemove = true;
            }
        }
    }

    void CheckPlayersState()
    {
        if (procDoCheckingPlayers == null)
            procDoCheckingPlayers = StartCoroutine(DoCheckPlayersState());
    }

    IEnumerator DoCheckPlayersState()
    {   //find most close one in the list
        CheckingPlayerState mostCloseState = null;
        CheckingPlayerState lastMostCloseState = null;
        while (checkingPlayerStates.Count != 0)
        {
            for (int i = 0; i < checkingPlayerStates.Count; i++)
            {
                var data = checkingPlayerStates[i];
                if (data.CanRemove)
                {
                    checkingPlayerStates.RemoveAt(i--);
                }
                else
                {
                    if (!data.isFollowing &&
                        ShouldRecongnize(data.player))
                    {
                        if (mostCloseState == null)
                        {
                            mostCloseState = data;
                        }
                        else
                        {
                            Transform[] list = new Transform[] { mostCloseState.player.transform, data.player.transform };
                            var closedTransform = Vector3Extentioner.GetMostClosedOne(modelPhysicsController.transform, list);
                            var shouldCloseOneChage = data.player.transform == closedTransform;
                            mostCloseState = shouldCloseOneChage ? data : mostCloseState;
                        }

                        var state = GetStateByDist(data.player.transform.position);
                    }
                }
            }

            if (mostCloseState != lastMostCloseState)
            {
                lastMostCloseState = mostCloseState;
                StartClosePlayerTracking(mostCloseState);
            }
            yield return new WaitForFixedUpdate();
        }

        procDoCheckingPlayers = null;
        yield return null;
    }

    void StartClosePlayerTracking(CheckingPlayerState data)
    {
        if (procDoTrackingClosePlayer != null)
            StopCoroutine(procDoTrackingClosePlayer);

        procDoTrackingClosePlayer = StartCoroutine(DoClosePlayerTracking(data));
    }
    IEnumerator DoClosePlayerTracking(CheckingPlayerState data)
    {
        checkingPlayerStates.ForEach(x => x.isFollowing = x == data);

        var beforeState = (StateKinds)state;
        var lastPlayerPosition = data.player.transform.position;
        var lastAPH = modelPhysicsController.actionPointHandler;
        while (lastAPH == modelPhysicsController.actionPointHandler)
        {
            yield return new WaitForFixedUpdate();

            if (!data.shouldRemove)
            {
                var isPositionChanged = lastPlayerPosition != data.player.transform.position;
                var nowState = GetStateByDist(data.player.transform.position);
                if (nowState != beforeState)
                {
                    beforeState = nowState;
                    ActionPointHandler aph = GetEachStateOfAPH(nowState, data);
                    SetAPH(aph);
                }

                if (isPositionChanged)
                {
                    modelPhysicsController.ChageLastAPPosition(data.player.transform);
                }
            }
        }

        SetState((int)StateKinds.Normal);
        procDoTrackingClosePlayer = null;
    }

    ActionPointHandler GetEachStateOfAPH(StateKinds kinds, CheckingPlayerState data)
    {
        ActionPointHandler aph = null;
        switch (kinds)
        {
            case StateKinds.Notice:
                aph = GetNoticeAPH(data);
                break;
        }

        return aph;
    }

    StateKinds GetStateByDist(Vector3 WPosition)
    {
        var dist = Vector3.Distance(modelPhysicsController.transform.position, WPosition);
        return dist == (float)StateByDist.Attack ? StateKinds.Attack : StateKinds.Notice;
    }

    ActionPointHandler GetNoticeAPH(CheckingPlayerState playerState)
    {
        var requireAPCount = 2;
        var apPooler = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP);
        var APs = new List<ActionPoint>();
        APs.Capacity = requireAPCount;

        for (int i = 0; i < requireAPCount; i++)
            APs.Add(apPooler.GetNewOne<PersonActionPoint>());

        var aph = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.APH).GetNewOne<ActionPointHandler>();
        aph.SetAPs(APs);
        aph.ShouldLoop = false;

        SetAPWithDuring(APs[0], playerState.player.transform, PersonActionPoint.StateKind.Surprize);
        SetAPWithDuring(APs[1], playerState.player.transform, PersonActionPoint.StateKind.LookAround, true, true);

        return aph;
    }

    void SetAPWithDuring(ActionPoint ap, Transform target, PersonActionPoint.StateKind kind, bool shouldChangeRotation = false, bool shouldChangePosition = false)
    {
        var length = aniController.GetLength(kind.ToString());
        ap.SetAPWithDuring(modelPhysicsController.transform, target, kind, length, shouldChangeRotation, shouldChangePosition);
    }
    class CheckingPlayerState
    {
        public Player player = null;
        public bool isFollowing = false;
        public bool shouldRemove = false;
        public bool CanRemove { get { return !isFollowing && shouldRemove; } }
        public CheckingPlayerState(Player player)
        {
            this.player = player;
        }
    }
}
