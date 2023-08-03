using UnityEngine;

internal class Sitting_PersonAniState : PersonAniState
{
    public const string SittingLevel = "SittingLevel";
    public enum SittingState { non, Ground, Low, Middle, High }
    public Sitting_PersonAniState(Animator aniController) : base(aniController)
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