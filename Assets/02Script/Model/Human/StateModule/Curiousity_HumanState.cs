using System;
using System.Collections;
using UnityEngine;

public class Curiousity_HumanState : HumanState
{
    enum State { HoldingWeapon, Curiousity, Non }
    State state = State.Non;
    float curiosityDIst = 0;
    const float MinCuriositiyDist = 3f;
    float curiosityTime = 0;
    const float MaxCuriosityTime = 3;
    bool IsCuriousState { get { return MinCuriositiyDist < curiosityTime && curiosityDIst < MaxCuriosityTime; } }
    bool isAPHDone = false;
    Coroutine procCountingIgnoreTime = null;
    AnimationPointHandler PlayingAPH { set; get; }
    public Curiousity_HumanState(Human person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null &&
                !IsCuriousState;
    }
    public override void EnterToException()
    {
        if (IsCuriousState)
        {
            SetNormalState();
        }
        // else
        // {
        //     SetState(StateKinds.HoldingWeapon, new PersonPrepareData(prepareData.target));
        // }
    }
    protected override void OnStartModule()
    {
        if (procCountingIgnoreTime != null) return;

        var targetMPH = prepareData.target;
        if (GetHoldState == InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState.Non)
        {
            state = State.HoldingWeapon;
            var aph = GetNewAPH(1);
            SetAPs(aph.animationPoints[0], prepareData.target, HumanAniState.StateKind.HoldingWeapon, 0, false, true);
            aph.animationPoints[0].animationPointData.whenAnimationStart += () => HandleWeapon(HumanAniState.StateKind.HoldingWeapon);
            SetAPH(aph, true);
        }
        else
        {
            state = State.Curiousity;
            var aph = GetNewAPH(2);
            SetAPs(aph.animationPoints[0], prepareData.target, HumanAniState.StateKind.Surprize, 0, false, true);
            SetAPs(aph.animationPoints[1], prepareData.target, HumanAniState.StateKind.LookAround, 0, true, false);
            procCountingIgnoreTime = StartCoroutine(IgnoreTimeByAnimation(aph));
            SetAPH(aph, true);
            StartTracingTargetInSight(targetMPH, () => isAPHDone);
        }
    }
    IEnumerator IgnoreTimeByAnimation(AnimationPointHandler aph)
    {
        yield return new WaitUntil(() => aph.index > 0);
        procCountingIgnoreTime = null;
    }

    protected override bool ShouldStopAfterCast(bool isHit)
    {
        // target find when aph running.
        if (isHit)
        {
            curiosityDIst = Vector3.Distance(ActorTransform.position, prepareData.target.position);

            if (IsCuriousState)
            {
                curiosityTime += Time.fixedDeltaTime;
            }
            else
            {
                SetState(StateKinds.Tracking, new PersonPrepareData(prepareData.target));
                return true;
            }
        }

        return false;
    }

    public override void Exit()
    {
        base.Exit();
        isAPHDone = false;
        curiosityTime = 0f;
        curiosityDIst = 0f;
        procCountingIgnoreTime = null;
        state = State.Non;
    }
    protected override void AfterAPHDone()
    {
        switch (state)
        {
            case State.HoldingWeapon:
                StartModule();
                break;

            case State.Curiousity:
                isAPHDone = true;
                SetNormalState();
                break;
        }

    }
}
