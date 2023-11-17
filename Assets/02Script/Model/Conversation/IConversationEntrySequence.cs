using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConversationEntrySequence
{
    public bool CanEnterConversation();
    public void PrepaerConversation(AnimationPointHandler waitingAPH, AnimationPointHandler startAPH, Action<IConversationEntrySequence> APHDoneAlert, Action<IConversationEntrySequence, ConversationEntry.SuddenEndedState> suddenEndedAlert);
    public void StartConversation();
    public void EndConversation();
    public void AlertHold();
    public void AlertUnHold();
    public void AlertCombat(Vector3 targetPosition);
    public void AlertNonResponce(Vector3 targetPosition);
}
