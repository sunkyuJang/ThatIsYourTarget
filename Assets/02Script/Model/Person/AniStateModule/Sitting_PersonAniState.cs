using UnityEngine;

internal class Sitting_PersonAniState : PersonAniState
{
    string SittingLevel { get { return "SittingLevel"; } }
    enum State { Ground = 1, Low, Middle, High }
    public Sitting_PersonAniState(Animator aniController) : base(aniController)
    {

    }

    public override void Enter()
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