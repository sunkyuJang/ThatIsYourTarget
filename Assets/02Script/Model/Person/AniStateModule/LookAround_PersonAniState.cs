using UnityEngine;

internal class LookAround_PersonAniState : PersonAniState
{
    string LookAround { get { return "LookAround"; } }
    public LookAround_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
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