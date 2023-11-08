using UnityEngine;

internal class Surprize_PersonAniState : PersonAniState
{
    string Surprize { get { return "ShouldSurprize"; } }
    public Surprize_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {
    }

    protected override void StartModule()
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