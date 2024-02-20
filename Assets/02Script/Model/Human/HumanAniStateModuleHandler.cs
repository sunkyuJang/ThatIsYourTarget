using UnityEngine;

public class HumanAniStateModuleHandler : StateModuleHandler
{
    public RagDollHandler RagDollHandler { private set; get; }
    public Animator Animator { private set; get; }
    public HumanAniStateModuleHandler(Animator animator, RagDollHandler ragDollHandler)
    {
        Animator = animator;
        RagDollHandler = ragDollHandler;
        modules = HumanAniState.GetStatesList(this);
    }

    public void EnterModule(HumanAniState.StateKind state, StateModule.PrepareData prepareData = null)
    {
        base.EnterModule((int)state, prepareData);
    }

    public HumanAniState GetModule(HumanAniState.StateKind state) => GetModule((int)state) as HumanAniState;

}
