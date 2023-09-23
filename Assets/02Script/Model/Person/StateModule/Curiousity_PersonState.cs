using System;
using System.Collections;
using UnityEngine;

public class Curiousity_PersonState : PersonState
{
    float curiosityDIst = 0;
    const float MinCuriositiyDist = 3f;
    float curiosityTime = 0;
    const float MaxCuriosityTime = 3;
    bool IsCuriousState { get { return curiosityTime < MaxCuriosityTime && curiosityDIst > MinCuriositiyDist; } }
    bool isAPHDone = false;
    Coroutine procCountingIgnoreTime = null;
    AnimationPointHandler PlayingAPH { set; get; }
    public Curiousity_PersonState(Person person) : base(person) { }
    private delegate Func<bool> conditionOfEndForTracking();
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
        else
        {
            SetState(StateKinds.DrawWeapon, new PersonPrepareData(prepareData.target));
        }
    }
    protected override void StartModule()
    {
        if (procCountingIgnoreTime != null) return;

        var targetMPH = prepareData.target;
        PlayingAPH = GetCuriousityAPH(targetMPH);
        procCountingIgnoreTime = StartCoroutine(IgnoreTimeByAnimation(PlayingAPH));
        SetAPH(PlayingAPH, true);
        TracingTargetInSightProcess(targetMPH, () => isAPHDone);
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
                SetState(StateKinds.DrawWeapon, new PersonPrepareData(prepareData.target));
                return true;
            }
        }

        return false;
    }

    private AnimationPointHandler GetCuriousityAPH(Transform target)
    {
        var aph = GetNewAPH(2);
        SetAPs(aph.animationPoints[0], target, PersonAniState.StateKind.Surprize, 0, false, true);
        SetAPs(aph.animationPoints[1], target, PersonAniState.StateKind.LookAround, 0, true, false);

        return aph;
    }

    public override void Exit()
    {
        isAPHDone = false;
        curiosityTime = 0f;
        curiosityDIst = 0f;
        procCountingIgnoreTime = null;
    }
    protected override void AfterAPHDone()
    {
        prepareData = null;
        isAPHDone = true;
        SetNormalState();
    }
}
