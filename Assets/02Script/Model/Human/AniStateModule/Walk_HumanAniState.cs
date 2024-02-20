using UnityEngine;

internal class Walk_HumanAniState : HumanAniState
{
    string walkLevel { get { return "WalkAroundLevel"; } }
    AnimationPointHandler.WalkingState state = AnimationPointHandler.WalkingState.Walk;
    public void SetWalkState(AnimationPointHandler.WalkingState state) => this.state = state;
    public Walk_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }

    public override bool IsReady()
    {
        return Animator != null && state >= 0;
    }

    protected override void StartModule()
    {
        Animator.SetInteger(walkLevel, (int)state);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }
}