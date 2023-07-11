using UnityEngine;

internal class TurnAround_PersonAniState : PersonAniState
{
    string TurnDegree { get { return "TurnDegree"; } }
    float defaultDegree = 361f;
    public TurnAround_PersonAniState(Animator aniController) : base(aniController)
    {
    }

    protected override void DoEnter()
    {
        Animator.SetFloat(TurnDegree, ap.targetDegree);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        Animator.SetFloat(TurnDegree, defaultDegree);
    }
}