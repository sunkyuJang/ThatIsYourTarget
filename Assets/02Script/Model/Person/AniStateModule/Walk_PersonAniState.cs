using UnityEngine;

internal class Walk_PersonAniState : PersonAniState
{
    string walkLevel { get { return "WalkAroundLevel"; } }
    AnimationPointHandler.WalkingState state = AnimationPointHandler.WalkingState.Walk;
    public void SetWalkState(AnimationPointHandler.WalkingState state) => this.state = state;
    public Walk_PersonAniState(Animator ani) : base(ani)
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