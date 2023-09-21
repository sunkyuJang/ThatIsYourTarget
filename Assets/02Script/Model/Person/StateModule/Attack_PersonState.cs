using UnityEngine;

public class Attack_PersonState : PersonState
{
    public Attack_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
        SetAPs(aph.animationPoints[0], prepareData.target, PersonAniState.StateKind.Attack, 0, false, true);
        aph.animationPoints[0].EventTrigger = AttackTrigger;
        SetAPH(aph, true);
    }

    public void IsPlayingTargetAnimation(AnimationClip targetClip)
    {

    }

    public void AttackTrigger(string code)
    {
        if (code == "Attack")
        {
            Debug.Log("call In");
        }
    }

    protected override StateKinds AfterAPHDone(out PersonPrepareData data)
    {
        data = prepareData;
        return StateKinds.Tracking;
    }
    public override void Exit() { }
}
