using UnityEngine;
using UnityEngine.UIElements;

public class Tracking_PersonState : PersonState
{
    enum State { UsingWeapon, Tracking, Non }
    State state = State.Non;
    bool isAphDone = false;
    bool shouldFixedLookAt = false;
    StateKinds stateKinds;
    AnimationPointHandler aph { set; get; } = null;
    Coroutine ProcessTracingTarget { get; set; } = null;
    public Tracking_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null && Weapon != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        if (ProcessTracingTarget == null)
        {
            if (state == State.Non)
            {
                Debug.Log(GetHoldState.ToString());
                if (GetHoldState != InteractionObjGrabRig.State.Using)
                {
                    Debug.Log("isin");
                    state = State.UsingWeapon;
                    var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
                    SetAPsImmediate(aph.GetAnimationPoint(0), PersonAniState.StateKind.UsingWeapon, 0f);
                    aph.GetAnimationPoint(0).whenAnimationStart += () => HandleWeapon(PersonAniState.StateKind.UsingWeapon);
                    Debug.Log("isin  " + aph.GetAnimationPoint(0).state);
                    SetAPH(aph, true);
                    return;
                }
                else
                {
                    Debug.Log("isin2");
                    state = State.Tracking;
                    StartModule();
                    return;
                }
            }
            else
            {
                Debug.Log("isin3");
                state = State.Tracking;
                aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
                aph.shouldLoop = true;
                SetAPs(aph.GetAnimationPoint(0), prepareData.target, PersonAniState.StateKind.LookAround, 0f, true, true);
                isAphDone = false;
                SetAPH(aph, true);
                StartTracingTargetInSight(prepareData.target, () => isAphDone);
            }
        }
    }

    AnimationPointHandler GetTrackingAPH()
    {


        return aph;
    }

    protected override bool ShouldStopAfterCast(bool isHit)
    {
        // this function will loop untill isAphDone == true
        aph.shouldLoop = false;
        var ap = aph.GetAnimationPoint(0);
        if (isHit)
        {
            var dist = Vector3.Distance(ActorTransform.position, prepareData.target.position);
            if (dist < Weapon.range)
            {
                SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0, false, true);
                stateKinds = StateKinds.Attack;
                return true;
            }
            else
            {
                stateKinds = StateKinds.Tracking;
                SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0, true, true);
                aph.shouldLoop = true;
            }
        }
        else
        {
            stateKinds = StateKinds.Patrol;
            SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, 0, false, true);
            return true;
        }

        return false;
    }

    public override void Exit()
    {
        isAphDone = true;
        ProcessTracingTarget = null;
        state = State.Non;
        base.Exit();
    }
    protected override void AfterAPHDone()
    {
        if (state == State.UsingWeapon)
        {
            Debug.Log("isin using done");
            StartModule();
        }
        switch (stateKinds)
        {
            case StateKinds.Attack: SetState(StateKinds.Attack, prepareData); break;
            case StateKinds.Patrol: SetState(StateKinds.Patrol, prepareData); break;
            case StateKinds.Tracking: StartModule(); break;
        }
    }
}