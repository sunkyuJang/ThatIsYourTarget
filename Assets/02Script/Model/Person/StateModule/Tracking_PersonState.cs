using UnityEngine;

public class Tracking_PersonState : PersonState
{
    bool isAphDone = false;
    bool shouldFixedLookAt = false;
    StateKinds stateKinds;
    AnimationPointHandler aph { set; get; } = null;
    public Tracking_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        var weapon = Weapon;

        if (weapon != null)
        {
            aph = GetTrackingAPH();
            SetAPH(aph, true);
            isAphDone = false;

            TracingTargetInSightProcess(prepareData.target, () => isAphDone);
        }
    }

    AnimationPointHandler GetTrackingAPH()
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
        aph.shouldLoop = true;
        var ap = aph.GetActionPoint(0);
        SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, 0f, true, true);

        return aph;
    }

    protected override bool ShouldStopAfterCast(bool isHit)
    {
        // this function will loop untill isAphDone == true
        aph.shouldLoop = true;
        var ap = aph.GetActionPoint(0);
        if (isHit)
        {
            var dist = Vector3.Distance(ActorTransform.position, prepareData.target.position);
            if (dist < Weapon.Range)
            {
                SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0, false, true);
                stateKinds = StateKinds.Attack;
                aph.shouldLoop = false;
                return true;
            }
            else
            {
                stateKinds = StateKinds.Tracking;
                SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, 0, true, true);
            }
        }
        else
        {
            stateKinds = StateKinds.Patrol;
            SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, 0, false, true);
        }

        return false;
    }

    public override void Exit()
    {
        isAphDone = true;
        base.Exit();
    }
    protected override void AfterAPHDone()
    {
        switch (stateKinds)
        {
            case StateKinds.Attack: SetState(StateKinds.Attack, prepareData); break;
            case StateKinds.Patrol: SetState(StateKinds.Patrol, prepareData); break;
            case StateKinds.Tracking: StartModule(); break;
        }
    }
}