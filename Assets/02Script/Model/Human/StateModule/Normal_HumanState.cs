public class Normal_HumanState : HumanState
{
    public Normal_HumanState(Human person) : base(person)
    {

    }
    public override bool IsReady()
    {
        return true;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        // handle weapon state
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Walk);
        var ap = aph.GetAnimationPoint(0);
        var state = shouldOnGuard ? HumanAniState.StateKind.HoldingWeapon : HumanAniState.StateKind.KeepingWeapon;
        SetAPsImmediate(ap, state, 0);
        ap.animationPointData.whenAnimationStart += () => { HandleWeapon(state); };
        SetAPH(aph, true);
    }

    protected override void AfterAPHDone()
    {
        SetAPH();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
