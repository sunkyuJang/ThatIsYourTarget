using System.Collections.Generic;

public partial class StateModuleHandler
{
    protected List<StateModule> modules = new List<StateModule>();
    protected int playingModuleIndex = -1;
    //private StateModule playingModule = null;
    public void EnterModule(int targetModuleIndex, bool shouldTurnOffOldModule = true)
    {
        var playingModule = GetModule(playingModuleIndex);
        var targetModule = GetModule(targetModuleIndex);

        if (!modules.Exists(x => x == targetModule)) return;

        if (targetModule == null || targetModule == playingModule) return;

        if (!targetModule.IsReadyForEnter()) return;

        if (shouldTurnOffOldModule)
            playingModule?.Exit();

        playingModuleIndex = targetModuleIndex;
        playingModule.Enter();
    }

    public void StopPlayingModule()
    {
        GetModule(playingModuleIndex).Exit();
    }

    public StateModule GetModule(int num)
    {
        if (num < modules.Count)
            return modules[num];
        else
        {
            UnityEngine.Debug.Log("Target Module Couldnt found");
            return null;
        }
    }

    public bool IsSameModule(int num)
    {
        return num == playingModuleIndex;
    }
}

