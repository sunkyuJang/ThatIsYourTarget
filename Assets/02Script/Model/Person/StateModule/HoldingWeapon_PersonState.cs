using RootMotion.Demos;
using UnityEngine;

public class HoldingWeapon_PersonState : PersonState
{
    public static float AbsoluteAttackDist { set; get; } = 1f;
    public HoldingWeapon_PersonState(Person person) : base(person) { }

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
        aph.GetAnimationPoint(0).animationPointData.InteractionObj = Weapon;
        SetAPsImmediate(aph.GetAnimationPoint(0), PersonAniState.StateKind.HoldingWeapon, 0);
        aph.GetAnimationPoint(0).animationPointData.whenAnimationStart += () => HandleWeapon(PersonAniState.StateKind.HoldingWeapon);
        return aph;
    }
    protected override void AfterAPHDone()
    {
        var state = StateKinds.Tracking;
        var data = new PersonPrepareData(prepareData.target);
        SetState(state, data);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
