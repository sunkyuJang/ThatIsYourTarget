using JExtentioner;
using System.Collections;
using UnityEngine;

public class Curiousity_PersonState : PersonState
{
    int curiosityCnt = 0;
    const int MaxCuriosityCnt = 3;
    float warningTime = 0f;
    const float maxWarningTime = 3f;
    bool isAPHDone = false;
    Coroutine procCountingTime = null;
    Coroutine procCountingIgnoreTime = null;
    AnimationPointHandler PlayingAPH { set; get; }
    public Curiousity_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null &&
                curiosityCnt < MaxCuriosityCnt;
    }
    public override void EnterToException()
    {
        if (curiosityCnt < MaxCuriosityCnt)
        {
            SetNormalState();
        }
        else
        {
            SetState(StateKinds.PrepareAttack, new PersonPrepareData(prepareData.target));
        }
    }
    protected override void StartModule()
    {
        if (PlayingAPH != null) return;

        PlayingAPH = GetCuriousityAPH(prepareData.target.modelPhysicsHandler.transform);
        // 하는 중. 
        //첫 애니메이션 작동시 model이 움직이면서 콜라이터의 connect 및 remove가 지속적으로 발생하는 것을 막기위해
        //척 애니메이션이 동작하는 동안 만큼은 connect 및 remove를 통해 입력된 값을 무시하도록 한다.
        if (procCountingIgnoreTime != null) return;

        procCountingIgnoreTime = person.StartCoroutine(IgnoreTime(PlayingAPH));
        person.SetAPH(PlayingAPH, AfterAPHDone);
        if (procCountingTime != null)
        {
            person.StopCoroutine(procCountingTime);
        }

        procCountingTime = person.StartCoroutine(CountingTime(prepareData.target.modelPhysicsHandler.transform));
    }
    IEnumerator IgnoreTime(AnimationPointHandler aph)
    {
        yield return new WaitUntil(() => aph.index > 0);
        procCountingIgnoreTime = null;
    }

    void StartTracing(Transform target, APHManager aph)
    {
        var trackingProcess = GetTracingTargetInSightProcess(target.transform, () => isAPHDone);
    }
    IEnumerator CountingTime(Transform target)
    {
        var trackingProcess = GetTracingTargetInSightProcess(target.transform, () => isAPHDone);
        yield return WaitUntilExtentioner.WaitUntilWithFixedTime(() => isAPHDone);

        person.StopCoroutine(trackingProcess);
        procCountingTime = null;

        yield break;
    }

    protected override void WhenTargetInSight(bool isHit)
    {
        if (isHit)
        {

        }
        else
        {

        }
    }

    private AnimationPointHandler GetCuriousityAPH(Transform target)
    {
        var aph = person.GetNewAPH(2);
        person.SetAPs(aph.animationPoints[0], target, PersonAniState.StateKind.Surprize, true, 0, false, true);
        person.SetAPs(aph.animationPoints[1], target, PersonAniState.StateKind.LookAround, true, 0, true, false);

        return aph;
    }

    public override void Exit()
    {
        isAPHDone = false;
        warningTime = 0;
        procCountingTime = null;
        curiosityCnt = 0;
    }
    protected override StateKinds DoAfterDone(out PersonPrepareData prepareData)
    {
        prepareData = null;
        isAPHDone = true;
        return StateKinds.Normal;
    }
}
