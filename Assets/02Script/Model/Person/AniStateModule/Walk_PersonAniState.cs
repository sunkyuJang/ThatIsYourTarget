using System;
using UnityEngine;

internal class Walk_PersonAniState : PersonAniState
{
    Action<int> intAction;
    string walkLevel { get { return "WalkAroundLevel"; } }
    enum SubState { Walk, Run, Non }
    public Walk_PersonAniState(Animator ani) : base(ani)
    {

    }

    public void SetAction(Action<int> action) => intAction = action;

    public override bool IsReadyForEnter()
    {
        return base.IsReadyForEnter() && intAction != null;
    }

    public override void Enter()
    {
        Animator.SetInteger(walkLevel, ap.subState_int);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        intAction((int)SubState.Non);
        intAction = null;
    }
}