using System;
using UnityEngine;

internal class Walk_PersonAniState : PersonAniState
{
    Action<int> intAction;
    enum SubState { Walk, Run, Non }
    public Walk_PersonAniState(PersonAniController aniController) : base(aniController)
    {

    }

    public void SetAction(Action<int> action) => intAction = action;

    public override bool IsReadyForEnter()
    {
        return base.IsReadyForEnter() && intAction != null;
    }

    public override void Enter()
    {
        intAction(ap.subState_int);
    }

    public override void EnterToException()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}