using UnityEngine;

internal class Surprize_PersonAniState : PersonAniState
{
    string Surprize { get { return "Surprize"; } }
    public Surprize_PersonAniState(Animator ani) : base(ani)
    {
    }

    public override void Enter()
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