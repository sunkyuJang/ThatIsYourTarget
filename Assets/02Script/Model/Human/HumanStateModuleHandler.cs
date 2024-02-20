using Unity.VisualScripting;
using UnityEngine;
public class HumanStateModuleHandler : StateModuleHandler
{
    public HumanStateModuleHandler(Human Human)
    {
        modules = HumanState.GetStatesList(Human);
    }
    public T GetModule<T>(HumanState.StateKinds kinds) where T : HumanState => GetModule(ConvertStateKindToInt(kinds)) as T;
    public HumanState GetModule(HumanState.StateKinds kinds) => GetModule(ConvertStateKindToInt(kinds)) as HumanState;
    public bool IsSameModule(HumanState.StateKinds kinds) => IsSameModule(ConvertStateKindToInt(kinds));
    private int ConvertStateKindToInt(HumanState.StateKinds kinds) => (int)kinds;
    public HumanState.StateKinds GetPlayingModuleStateKind() => (HumanState.StateKinds)GetPlayingModuleIndex();
    new public HumanState GetPlayingModule() => base.GetPlayingModule() as HumanState;
    public HumanState.StateKinds GetThisKind(HumanState HumanState)
    {
        var index = -1;
        var count = 0;
        modules.ForEach(x =>
        {
            count++;
            if (x == HumanState)
                index = count;
        });

        return (HumanState.StateKinds)index;
    }
    public void SetLockModuleChange(HumanState.StateKinds requestState, HumanState.StateKinds realseState)
    {
        base.SetLockModuleChange(ConvertStateKindToInt(requestState), ConvertStateKindToInt(realseState));
    }
    public bool isPlayingModuleHasTarget()
    {
        var prepareData = GetPlayingModule<HumanState>().prepareData;
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

    public Transform GetPlayingModuleTarget(HumanState.StateKinds exclusiveKinds)
    {
        var isStateKindDifferent = ConvertStateKindToInt(exclusiveKinds) != playingModuleIndex;
        if (isStateKindDifferent)
        {
            return GetPlayingModuleTarget();
        }

        return null;
    }
}
