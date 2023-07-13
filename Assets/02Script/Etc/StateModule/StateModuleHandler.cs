using System.Collections.Generic;

public partial class StateModuleHandler
{
    protected List<StateModule> modules = new List<StateModule>();
    private StateModule playingModule = null;
    public void EnterModule(int num, bool shouldTurnOffOldModule = true)
    {
        var targetModule = GetModule(num);
        if (targetModule != null)
            EnterModule(targetModule, shouldTurnOffOldModule);
    }

    protected void EnterModule(StateModule targetModule, bool shouldTurnOffOldModule = true)
    {
        if (!modules.Exists(x => x == targetModule)) return;

        if (targetModule == null || targetModule == playingModule) return;

        if (!targetModule.IsReadyForEnter()) return;

        if (shouldTurnOffOldModule)
            playingModule?.Exit();

        playingModule = targetModule;
        playingModule.Enter();
    }

    public void StopPlayingModule()
    {
        playingModule.Exit();
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
        if (playingModule == null) return false;

        var targetModule = GetModule(num);
        return targetModule != null && targetModule == playingModule;
    }
}

