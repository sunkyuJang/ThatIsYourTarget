using System;
using UnityEngine;

public class Dead_PersonState : PersonState
{
    new DeadPrepareData prepareData { get { return base.prepareData as DeadPrepareData; } }
    public Dead_PersonState(Person person) : base(person)
    {

    }
    public override bool IsReady()
    {
        return true;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Walk);
        SetAPsImmediate(aph.GetActionPoint(0), PersonAniState.StateKind.Dead, -1f);
        SetAPH(aph, true);
        ModuleHandler.SetLockModuleChange(StateKinds.Dead, StateKinds.Non);
    }

    protected override void AfterAPHDone()
    {
        Exit();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public class DeadPrepareData : PersonPrepareData
    {
        Action WhenAnimationEnd { set; get; }

        public DeadPrepareData(Transform target, Action whenAnimationEnd) : base(target)
        {
            WhenAnimationEnd = whenAnimationEnd;
        }
    }
}
