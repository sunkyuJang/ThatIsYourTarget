using System.Collections.Generic;

public partial class StateModuleHandler
{
    protected List<StateModule> modules = new List<StateModule>();
    protected int playingModuleIndex = -1;
    public void EnterModule(int targetModuleIndex, StateModule.PrepareData prepareData = null)
    {
        var playingModule = GetModule(playingModuleIndex);
        var targetModule = GetModule(targetModuleIndex);

        if (!modules.Exists(x => x == targetModule)) return;

        if (targetModule == null || targetModule == playingModule) return;

        if (targetModule.TryEnter(prepareData))
        {
            playingModule?.Exit();
            playingModuleIndex = targetModuleIndex;
        }
    }

    public void StopPlayingModule()
    {
        GetModule(playingModuleIndex).Exit();
    }

    public StateModule GetModule(int num)
    {
        if (num < 0) return null;

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
    public int GetPlayingModuleIndex() => playingModuleIndex;
    public StateModule GetPlayingModule() => GetModule(playingModuleIndex);
    public T GetPlayingModule<T>() where T : StateModule => GetPlayingModule() as T;
    public virtual bool HasPrepareData()
    {
        var playingModule = GetPlayingModule();
        return playingModule != null && playingModule.prepareData != null;
    }
}

