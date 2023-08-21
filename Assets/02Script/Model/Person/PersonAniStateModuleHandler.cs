using UnityEngine;

public class PersonAniStateModuleHandler : StateModuleHandler
{
    public PersonAniStateModuleHandler(Animator animator)
    {
        modules = PersonAniState.GetStatesList(animator);
    }

    public void EnterModule(PersonAniState.StateKind state, StateModule.PrepareData prepareData = null)
    {
        base.EnterModule((int)state, prepareData);
    }

    public PersonAniState GetModule(PersonAniState.StateKind state) => GetModule((int)state) as PersonAniState;

}
