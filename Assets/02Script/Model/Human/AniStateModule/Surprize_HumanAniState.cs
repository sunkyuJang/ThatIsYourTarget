using UnityEngine;

internal class Surprize_HumanAniState : HumanAniState
{
    string Surprize { get { return "ShouldSurprize"; } }
    public Surprize_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
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