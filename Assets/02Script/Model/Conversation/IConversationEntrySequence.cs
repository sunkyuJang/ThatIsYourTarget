using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConversationSequence
{
    public bool CanEnterConversation();
    public void PrepaerConversation(AnimationPointHandler waitingAPH, AnimationPointHandler startAPH, Action<IConversationSequence> APHDoneAlert, Action<IConversationSequence, ConversationEntry.SuddenEndedState> suddenEndedAlert);
    public void StartConversation();
    public void EndConversation();
    public void AlertHold();
    public void AlertUnHold();
    public void AlertCombat(Vector3 targetPosition);
    public void AlertNonResponce(Vector3 targetPosition);
}
