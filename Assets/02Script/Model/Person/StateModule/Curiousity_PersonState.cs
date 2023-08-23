using System.Collections;
using UnityEngine;

public class Curiousity_PersonState : PersonState
{
    float curiosityTime = 0;
    const float MaxCuriosityTime = 3;
    bool IsCuriousState { get { return curiosityTime < MaxCuriosityTime; } }
    bool isAPHDone = false;
    Coroutine procCountingIgnoreTime = null;
    AnimationPointHandler PlayingAPH { set; get; }
    public Curiousity_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null &&
                IsCuriousState;
    }
    public override void EnterToException()
    {
        if (IsCuriousState)
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
        if (procCountingIgnoreTime != null) return;

        PlayingAPH = GetCuriousityAPH(prepareData.target.modelPhysicsHandler.transform);
        procCountingIgnoreTime = person.StartCoroutine(IgnoreTimeByAnimation(PlayingAPH));
        person.SetAPH(PlayingAPH, AfterAPHDone);
        TracingTargetInSightProcess(prepareData.target.transform, () => isAPHDone);
    }
    IEnumerator IgnoreTimeByAnimation(AnimationPointHandler aph)
    {
        yield return new WaitUntil(() => aph.index > 0);
        procCountingIgnoreTime = null;
    }

    protected override void WhenTargetInSight(bool isHit)
    {
        // target find when aph running.
        if (isHit)
        {
            var dist = person.modelPhysicsHandler.GetDistTo(prepareData.target.modelPhysicsHandler.transform);
            if (dist > PrepareAttack_PersonState.prepareAttackDist
                && IsCuriousState)
            {
                curiosityTime += Time.fixedTime;
            }
            else
            {
                SetState(StateKinds.PrepareAttack, new PersonPrepareData(prepareData.target));
            }
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
        curiosityTime = 0f;
        procCountingIgnoreTime = null;
    }
    protected override StateKinds DoAfterAPHDone(out PersonPrepareData prepareData)
    {
        prepareData = null;
        isAPHDone = true;
        return StateKinds.Normal;
    }
}
