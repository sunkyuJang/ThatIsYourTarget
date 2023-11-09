using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonConversationHandler : ConversationHandler
{
    new PersonStateModuleHandler ModuleHandler { get { return base.ModuleHandler as PersonStateModuleHandler; } }
    protected override bool OnCanEnterConversation()
    {
        var state = ModuleHandler.GetPlayingModuleStateKind();
        return state == PersonState.StateKinds.Normal;
    }

    protected override void OnPrepaerConversation()
    {
        base.OnPrepaerConversation();
        var data = ConversationEntry.GetData(this);
        Model.SetAPH(data.waitingAph, () => { });
        Model.ModuleHandler.SetWhenStateChange(WhenSuddenEndedConversation);
    }

    protected override void OnStartConversation(Action<IConversationEntrySequence> APHDoneAlert, Action<IConversationEntrySequence, bool> suddenEndedAlert)
    {
        base.OnStartConversation(APHDoneAlert, suddenEndedAlert);
        var data = ConversationEntry.GetData(this);
        Model.SetAPH(data.startAph, () => { WhenAPHDone(APHDoneAlert); });
        Model.ModuleHandler.SetWhenStateChange(WhenSuddenEndedConversation);
    }

    protected override void OnEndConversation()
    {
        base.OnEndConversation();
        Model.SetAPH();
    }

    protected void WhenSuddenEndedConversation()
    {
        OnSuddenEndedConversation(this, ModuleHandler.GetPlayingModuleStateKind() != PersonState.StateKinds.Dead);
    }

    protected void WhenAPHDone(Action<IConversationEntrySequence> APHDoneAlert)
    {
        APHDoneAlert?.Invoke(this);
    }

    protected override void OnSuddenEndedConversation(IConversationEntrySequence conversationEntrySequence, bool canResponding)
    {
        base.OnSuddenEndedConversation(conversationEntrySequence, canResponding);
    }
}
