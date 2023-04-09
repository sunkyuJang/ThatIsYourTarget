using UnityEngine;

internal class TurnAround_PersonAniState : PersonAniState
{
    string TurnAround { get { return "TurnAround"; } }
    float defaultDegree = 361f;
    public TurnAround_PersonAniState(Animator aniController) : base(aniController)
    {
    }

    public override void Enter()
    {
        Animator.SetFloat(TurnAround, ap.subState_float);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        Animator.SetFloat(TurnAround, defaultDegree);
    }
}