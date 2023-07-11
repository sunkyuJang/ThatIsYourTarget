using UnityEngine;

internal class Surprize_PersonAniState : PersonAniState
{
    string Surprize { get { return "ShouldSurprize"; } }
    public Surprize_PersonAniState(Animator ani) : base(ani)
    {
    }

    protected override void DoEnter()
    {
        Animator.SetBool(Surprize, true);
    }

    public override void EnterToException()
    {
    }

    public override void Exit()
    {
        Animator.SetBool(Surprize, false);
    }
}