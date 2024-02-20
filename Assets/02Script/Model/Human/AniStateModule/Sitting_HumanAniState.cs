using UnityEngine;

internal class Sitting_HumanAniState : HumanAniState
{
    public const string SittingLevel = "SittingLevel";
    public enum SittingState { non, Ground, Low, Middle, High }
    public Sitting_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }

    protected override void StartModule()
    {
        Animator.SetInteger(SittingLevel, ap.subState_int);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        Animator.SetInteger(SittingLevel, 0);
    }
}