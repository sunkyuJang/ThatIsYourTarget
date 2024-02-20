using UnityEngine;
public class Dead_HumanAniState : HumanAniState
{
    public Dead_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
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