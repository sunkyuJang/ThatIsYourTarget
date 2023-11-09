using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class ConversationHandler : IConversationEntrySequence
{
    public Model Model { set; get; }
    public StateModuleHandler ModuleHandler { get { return Model.ModuleHandler; } }
    protected ConversationEntry ConversationEntry { set; get; }
    public bool CanEnterConversation()
    {
        return OnCanEnterConversation();
    }

    public void PrepaerConversation(ConversationEntry conversationEntryInfo)
    {
        ConversationEntry = conversationEntryInfo;
        OnPrepaerConversation();
    }

    public void StartConversation(Action<IConversationEntrySequence> APHDoneAlert, Action<IConversationEntrySequence, bool> suddenEndedAlert)
    {
        OnStartConversation(APHDoneAlert, suddenEndedAlert);
    }

    public void EndConversation()
    {
        OnEndConversation();
    }

    public void SuddenEndedConversation(Model model, bool canResponding)
    {
        SuddenEndedConversation(model, canResponding);
    }

    protected virtual bool OnCanEnterConversation() { return false; }
    protected virtual void OnPrepaerConversation() { }
    protected virtual void OnStartConversation(Action<IConversationEntrySequence> APHDoneAlert, Action<IConversationEntrySequence, bool> SuddenEndedAlert) { }
    protected virtual void OnEndConversation() { }
    protected virtual void OnSuddenEndedConversation(IConversationEntrySequence model, bool canResponding) { }
}
