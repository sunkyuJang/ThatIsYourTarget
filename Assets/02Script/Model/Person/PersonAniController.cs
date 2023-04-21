using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMath;

public class PersonAniController : AniController
{
    public GameObject personNeck;
    // private List<StateKind> playStandList = new List<StateKind>()
    // {
    //     StateKind.Standing, StateKind.PrepareAttack, StateKind.TurnHead
    // };

    protected Dictionary<PersonAniState.StateKind, PersonAniState> StateModule { set; get; }
    protected override void Start()
    {
        base.Start();
        StateModule = PersonAniState.GetNewStateList(animator);
        bodyThreshold = 80f;
    }
    protected override bool IsWalkState()
    {
        return
            animator.GetCurrentAnimatorStateInfo(0).IsName("WalkAround") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("RunningAround");
    }

    protected override void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false)
    {
        if (actionPoint is PersonActionPoint)
        {
            var ap = actionPoint as PersonActionPoint;

            if (StateModule.ContainsKey(ap.State))
            {
                var module = StateModule[ap.State];
                module.SetAP(ap);

                if (module.IsReadyForEnter())
                {
                    module.Enter();
                }
                else
                {
                    module.EnterToException();
                }

                // walk animation should stop for other animation
                SetWalkModule(ActionPointHandler.WalkingState.Non);

                StartAniTimeCount(ap, shouldReturnAP, module);
            }
        }
    }

    void SetWalkModule(ActionPointHandler.WalkingState walkingState)
    {
        var module = StateModule[PersonAniState.StateKind.Walk] as Walk_PersonAniState;
        if (module != null)
        {
            module.SetWalkState(walkingState);
            module.Enter();
        }
        else
        {
            Debug.Log("there has no walk module");
        }
    }

    public void SetTurnHead(ActionPoint ap)
    {
        headFollowTarget = ap.transform;
    }
    protected override IEnumerator DoResetAni(bool shouldReadNextAction, StateModule stateModule = null)
    {
        if (stateModule == null)
        {
            // activate reset module.
        }
        else
        {
            if (stateModule is PersonAniState)
            {
                (stateModule as PersonAniState).Exit();
            }
        }

        SetWalkModule(walkingState);
        StartCoroutine(base.DoResetAni(shouldReadNextAction, null));

        yield return null;
    }

    protected override float GetMakeTurnDuring(float degree)
    {
        var ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonActionPoint>();
        ap.State = PersonAniState.StateKind.TurnAround;
        ap.targetDegree = degree;
        ap.during = ap.GetLength(GetStateNameByDegree(ap.targetDegree));
        StartAni(ap, true);
        return ap.during;
    }

    string GetStateNameByDegree(float degree)
    {
        if (degree >= 0)
        {
            return degree > 135f ? "LongTurnR" : "TurnR";
        }
        else
        {
            return degree < -135f ? "TurnL" : "LongTurnL";
        }
    }

    // public ActionPoint MakeHeadTurn()
    // {
    //     var ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonActionPoint>();
    //     ap.State = PersonAniController.StateKind.TurnHead;
    //     ap.during = 3f;
    //     StartAni(ap, true);

    //     return ap;
    // }
}
