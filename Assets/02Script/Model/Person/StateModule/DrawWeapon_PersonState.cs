using UnityEngine;

public class DrawWeapon_PersonState : PersonState
{
    public static float AbsoluteAttackDist { set; get; } = 1f;
    public DrawWeapon_PersonState(Person person) : base(person) { }

    public override bool IsReady()
    {
        return prepareData != null;
    }
    protected override void StartModule()
    {
        SetAPH(GetAttckAPHHandler(), true);
    }
    public override void EnterToException()
    {
        SetNormalState();
        Debug.Log("there have some problem");
    }

    public AnimationPointHandler GetAttckAPHHandler()
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
        (aph.GetActionPoint(0) as PersonAnimationPoint).Weapon = Weapon;
        SetAPs(aph.GetActionPoint(0), prepareData.target, PersonAniState.StateKind.DrawWeapon, 0, false, true);
        HoldWeapon(true);
        return aph;
    }
    protected override StateKinds AfterAPHDone(out PersonPrepareData data)
    {
        var state = StateKinds.Tracking;
        data = new PersonPrepareData(prepareData.target);
        return state;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
