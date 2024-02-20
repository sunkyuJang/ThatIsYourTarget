using UnityEngine;

internal class LookAround_HumanAniState : HumanAniState
{
    string LookAround { get { return "LookAround"; } }
    public LookAround_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }

    protected override void StartModule()
    {
        Animator.SetBool(LookAround, true);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        Animator.SetBool(LookAround, false);
    }
}