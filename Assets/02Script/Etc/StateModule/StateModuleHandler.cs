using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public partial class StateModuleHandler
{
    protected List<StateModule> modules = new List<StateModule>();
    protected int playingModuleIndex = -1;
    protected Action WhenStateChanged { set; get; }
    protected ModuleHandlerLock HandlerLock { set; get; }
    protected void SetLockModuleChange(int requestStateNum, int realseStateNum)
    {
        HandlerLock = new ModuleHandlerLock() { RequestStateNum = requestStateNum, RealseStateNum = realseStateNum };
    }
    public void EnterModule(int targetModuleIndex, StateModule.PrepareData prepareData = null, Action whenStateChanged = null)
    {
        if (HandlerLock != null)
        {
            if (targetModuleIndex == HandlerLock.RealseStateNum)
                HandlerLock = null;
            else return;
        }

        var playingModule = GetModule(playingModuleIndex);
        var targetModule = GetModule(targetModuleIndex);

        if (!modules.Exists(x => x == targetModule)) return;

        if (targetModule == null || targetModule == playingModule) return;

        if (targetModule.TryEnter(prepareData))
        {
            playingModule?.Exit();
            playingModuleIndex = targetModuleIndex;

            WhenStateChanged?.Invoke();
            WhenStateChanged = whenStateChanged;
        }
    }

    public void InterruptStateModule(StateModule targetModule, StateModule.PrepareData prepareData = null, Action whenStateChanged = null)
    {
        if (targetModule.TryEnter(prepareData))
        {
            StopPlayingModule();
            playingModuleIndex = -1;

            WhenStateChanged?.Invoke();
            WhenStateChanged = whenStateChanged;
        }
    }

    public void SetWhenStateChange(Action action) => WhenStateChanged = action;

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

    public class ModuleHandlerLock
    {
        public int RequestStateNum { set; get; } = -1;
        public int RealseStateNum { set; get; } = -1;
    }
}

