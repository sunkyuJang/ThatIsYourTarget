using System.Collections.Generic;

public partial class StateModuleHandler
{
    protected List<StateModule> modules = new List<StateModule>();
    private StateModule playingModule = null;
    public void EnterModule(int num, bool shouldTurnOffOldModule = true)
    {
        var targetModule = GetModule(num);

        if (targetModule == null || targetModule == playingModule) return;

        if (targetModule is IPrepareStateModule prepareStateModule && !prepareStateModule.IsPrepared()) return;

        if (shouldTurnOffOldModule)
            playingModule.Exit();

        playingModule = targetModule;
        playingModule.Enter();
    }

    public void StopPlayingModule()
    {
        playingModule.Exit();
    }

    public StateModule GetModule(int num) { return modules[num]; }

    public virtual bool SetPrepareData(int num, IPrepareModuleData data)
    {
        var module = GetModule(num) as IPrepareStateModule;
        if (module != null)
        {
            module.SetPrepare(data);
            return true;
        }

        return false;
    }
}

