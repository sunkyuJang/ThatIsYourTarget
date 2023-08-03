using System.Collections.Generic;

public partial class StateModuleHandler
{
    protected List<StateModule> modules = new List<StateModule>();
    protected int playingModuleIndex = -1;
    public virtual void EnterModule(int targetModuleIndex, StateModule.PrepareData prepareData = null)
    {
        var playingModule = GetModule(playingModuleIndex);
        var targetModule = GetModule(targetModuleIndex);

        if (!modules.Exists(x => x == targetModule)) return;

        if (targetModule == null || targetModule == playingModule) return;

        if (!targetModule.IsReady()) return;

        playingModule?.Exit();

        playingModuleIndex = targetModuleIndex;
        playingModule.Enter(prepareData);
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

