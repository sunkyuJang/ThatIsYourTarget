using Unity.VisualScripting;
using UnityEngine;
public class PersonStateModuleHandler : StateModuleHandler
{
    public PersonStateModuleHandler(Person person)
    {
        modules = PersonState.GetStatesList(person);
    }
    public T GetModule<T>(PersonState.StateKinds kinds) where T : PersonState => GetModule(ConvertStateKindToInt(kinds)) as T;
    public PersonState GetModule(PersonState.StateKinds kinds) => GetModule(ConvertStateKindToInt(kinds)) as PersonState;
    public bool IsSameModule(PersonState.StateKinds kinds) => IsSameModule(ConvertStateKindToInt(kinds));
    private int ConvertStateKindToInt(PersonState.StateKinds kinds) => (int)kinds;
    public PersonState.StateKinds GetPlayingModuleStateKind() => (PersonState.StateKinds)GetPlayingModuleIndex();
    new public PersonState GetPlayingModule() => base.GetPlayingModule() as PersonState;
    public PersonState.StateKinds GetThisKind(PersonState personState)
    {
        var index = -1;
        var count = 0;
        modules.ForEach(x =>
        {
            count++;
            if (x == personState)
                index = count;
        });

        return (PersonState.StateKinds)index;
    }
    public void SetLockModuleChange(PersonState.StateKinds requestState, PersonState.StateKinds realseState)
    {
        base.SetLockModuleChange(ConvertStateKindToInt(requestState), ConvertStateKindToInt(realseState));
    }
    public bool isPlayingModuleHasTarget()
    {
        var prepareData = GetPlayingModule<PersonState>().prepareData;
        return prepareData != null && prepareData.target != null;
    }

    public Transform GetPlayingModuleTarget()
    {
        if (isPlayingModuleHasTarget())
        {
            return GetPlayingModule().prepareData.target;
        }
        return null;
    }

    public Transform GetPlayingModuleTarget(PersonState.StateKinds exclusiveKinds)
    {
        var isStateKindDifferent = ConvertStateKindToInt(exclusiveKinds) != playingModuleIndex;
        if (isStateKindDifferent)
        {
            return GetPlayingModuleTarget();
        }

        return null;
    }
}
