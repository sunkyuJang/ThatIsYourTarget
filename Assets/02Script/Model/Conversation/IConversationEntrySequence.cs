using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConversationEntrySequence
{
    public bool CanEnterConversation();
    public void PrepaerConversation(ConversationEntry conversationEntryInfo);
    public void StartConversation(Action<IConversationEntrySequence> APHDoneAlert, Action<IConversationEntrySequence, bool> suddenEndedAlert);
    public void EndConversation();
    public void SuddenEndedConversation(Model model, bool canResponding);
}
