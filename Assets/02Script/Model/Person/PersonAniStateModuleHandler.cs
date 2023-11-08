using UnityEngine;

public class PersonAniStateModuleHandler : StateModuleHandler
{
    public RagDollHandler RagDollHandler { private set; get; }
    public Animator Animator { private set; get; }
    public PersonAniStateModuleHandler(Animator animator, RagDollHandler ragDollHandler)
    {
        Animator = animator;
        RagDollHandler = ragDollHandler;
        modules = PersonAniState.GetStatesList(this);
    }

    public void EnterModule(PersonAniState.StateKind state, StateModule.PrepareData prepareData = null)
    {
        base.EnterModule((int)state, prepareData);
    }

    public PersonAniState GetModule(PersonAniState.StateKind state) => GetModule((int)state) as PersonAniState;

}
