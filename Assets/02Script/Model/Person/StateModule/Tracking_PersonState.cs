using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class Tracking_PersonState : PersonState
{
    enum State { UsingWeapon, Tracking, Non }
    State state = State.Non;
    bool isAphDone = false;
    bool shouldFixedLookAt = false;
    StateKinds nextState;
    AnimationPointHandler aph { set; get; } = null;
    Coroutine ProcessTracingTarget { get; set; } = null;
    public Tracking_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        Debug.Log("isIn tracking isRady? : " + (prepareData != null && Weapon != null));
        return prepareData != null && Weapon != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        Debug.Log("isIn tracking outside");
        if (ProcessTracingTarget == null)
        {
            Debug.Log("isIn tracking in");
            if (state == State.Non)
            {
                Debug.Log(GetHoldState.ToString());
                if (GetHoldState != InteractionObjGrabRig.State.Using)
                {
                    Debug.Log("isin usingState");
                    state = State.UsingWeapon;
                    var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
                    SetAPsImmediate(aph.GetAnimationPoint(0), PersonAniState.StateKind.UsingWeapon, 0f);
                    aph.GetAnimationPoint(0).animationPointData.whenAnimationStart += () => HandleWeapon(PersonAniState.StateKind.UsingWeapon);
                    SetAPH(aph, true);
                    return;
                }
                else
                {
                    Debug.Log("isin tracking");
                    state = State.Tracking;
                    StartModule();
                    return;
                }
            }
            else
            {
                Debug.Log("isin tracking loop");
                state = State.Tracking;
                aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
                aph.shouldLoop = true;

                var trackingAP = aph.GetAnimationPoint(0);
                SetAPs(trackingAP, prepareData.target, PersonAniState.StateKind.LookAround, 0f, true, true);
                var trackingAPData = trackingAP.animationPointData;
                trackingAPData.LookAtTransform = prepareData.target;
                isAphDone = false;
                SetAPH(aph, true);

                aph.GetAnimationPoint(0).animationPointData.whenAnimationStart += () => StartTracingTargetInSight(prepareData.target, () => isAphDone);
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
            var dist = Vector3.Distance(ActorTransform.position, prepareData.target.position);
            if (dist < Weapon.range)
            {
                Debug.Log("isin tarcking attack");
                SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0, false, true);
                nextState = StateKinds.Attack;
                return true;
            }
            else
            {
                Debug.Log("isin tarcking tracking");
                nextState = StateKinds.Tracking;
                SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0, true, true);
                aph.shouldLoop = true;
            }
        }
        else
        {
            Debug.Log("isin tarcking patrol");
            nextState = StateKinds.Patrol;
            SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, 0, false, true);
            ap.animationPointData.LookAtTransform = null;
            return true;
        }

        return false;
    }

    public override void Exit()
    {
        isAphDone = true;
        ProcessTracingTarget = null;
        state = State.Non;
        Debug.Log("isin tarcking exit");
        base.Exit();
    }
    protected override void AfterAPHDone()
    {
        Debug.Log("isin tracking aph done");
        if (state == State.UsingWeapon)
        {
            StartModule();
            Debug.Log("isin using done");
            return;
        }
        switch (nextState)
        {
            case StateKinds.Attack:
                Debug.Log("isin tracking to  attack");
                SetState(StateKinds.Attack, prepareData);
                break;
            case StateKinds.Patrol:
                Debug.Log("isin tracking to patrol");
                SetState(StateKinds.Patrol, prepareData);
                break;
            case StateKinds.Tracking:
                Debug.Log("isin tracking to tracking");
                StartModule();
                break;
        }
    }
}