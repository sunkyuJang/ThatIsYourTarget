using RootMotion.Demos;
using UnityEngine;

public class HoldingWeapon_HumanState : HumanState
{
    public static float AbsoluteAttackDist { set; get; } = 1f;
    public HoldingWeapon_HumanState(Human person) : base(person) { }

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
        SetAPsImmediate(aph.GetAnimationPoint(0), HumanAniState.StateKind.HoldingWeapon, 0);
        aph.GetAnimationPoint(0).animationPointData.whenAnimationStart += () => HandleWeapon(HumanAniState.StateKind.HoldingWeapon);
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
