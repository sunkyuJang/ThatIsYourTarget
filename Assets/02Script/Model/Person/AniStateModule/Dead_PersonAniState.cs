using UnityEngine;
public class Dead_PersonAniState : PersonAniState
{
    public Dead_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }
    protected override void StartModule()
    {
        Animator.enabled = false;
        ModuleHandler.RagDollHandler.BeRagDollState(true);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }

}