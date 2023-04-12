using System;
using UnityEngine;

internal class Walk_PersonAniState : PersonAniState
{
    string walkLevel { get { return "WalkAroundLevel"; } }
    ActionPointHandler.WalkingState state = ActionPointHandler.WalkingState.Walk;
    public void SetWalkState(ActionPointHandler.WalkingState state) => this.state = state;
    public Walk_PersonAniState(Animator ani) : base(ani)
    {

    }

    public override bool IsReadyForEnter()
    {
        return Animator != null && state >= 0;
    }

    public override void Enter()
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