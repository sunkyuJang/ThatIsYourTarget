using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class Tracking_HumanState : HumanState
{
    enum State { UsingWeapon, Tracking, Non }
    State state = State.Non;
    bool isAphDone = false;
    bool shouldFixedLookAt = false;
    StateKinds nextState;
    AnimationPointHandler aph { set; get; } = null;
    Coroutine ProcessTracingTarget { get; set; } = null;
    KeyValuePair<bool, Vector3> missingTarget { set; get; } = new KeyValuePair<bool, Vector3>(false, Vector3.zero);
    public Tracking_HumanState(Human person) : base(person) { }
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
                if (GetHoldState != InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState.Using)
                {
                    state = State.UsingWeapon;
                    var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
                    SetAPsImmediate(aph.GetAnimationPoint(0), HumanAniState.StateKind.UsingWeapon, 0f);
                    aph.GetAnimationPoint(0).animationPointData.whenAnimationStart += () => HandleWeapon(HumanAniState.StateKind.UsingWeapon);
                    SetAPH(aph, true);
                    return;
                }
                else
                {
                    state = State.Tracking;
                    StartModule();
                    return;
                }
            }
            else
            {
                state = State.Tracking;
                aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
                aph.shouldLoop = true;

                var trackingAP = aph.GetAnimationPoint(0);
                SetAPs(trackingAP, prepareData.target, HumanAniState.StateKind.Non, 0f, true, true);
                var trackingAPData = trackingAP.animationPointData;
                trackingAPData.LookAtTransform = prepareData.target;
                trackingAPData.whenAnimationStart += () => StartTracingTargetInSight(prepareData.target, () => isAphDone);
                isAphDone = false;
                SetAPH(aph, true);
            }
        }
    }

    protected override bool ShouldStopAfterCast(bool isHit)
    {
        // this function will loop untill isAphDone == true
        aph.shouldLoop = false;
        var ap = aph.GetAnimationPoint(0);
        if (isHit)
        {
            prepareData.lastDetectedStandPosition = prepareData.target.position;
            var dist = Vector3.Distance(ActorTransform.position, prepareData.target.position);
            if (dist < Weapon.range * 0.9f)
            {
                SetAPs(ap, prepareData.target, HumanAniState.StateKind.Non, 0, false, true);
                nextState = StateKinds.Attack;
                AfterAPHDone();
                return true;
            }
            else
            {
                nextState = StateKinds.Tracking;
                SetAPs(ap, prepareData.target, HumanAniState.StateKind.Non, 0, true, true);
                aph.shouldLoop = true;
            }
        }
        else
        {
            nextState = StateKinds.Patrol;
            SetAPs(ap, prepareData.lastDetectedStandPosition, HumanAniState.StateKind.Non, 0, false, true);
            ap.animationPointData.LookAtTransform = null;
            AfterAPHDone();
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
            StartModule();
            return;
        }
        switch (nextState)
        {
            case StateKinds.Attack:
                SetState(StateKinds.Attack, prepareData);
                break;
            case StateKinds.Patrol:
                SetState(StateKinds.Patrol, prepareData);
                break;
            case StateKinds.Tracking:
                StartModule();
                break;
        }
    }
}