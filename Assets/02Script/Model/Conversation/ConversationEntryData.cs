using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ConversationEntryData
{
    public Collider targetCollider;
    public IConversationEntrySequence conversationEntrySequence;
    public AnimationPointHandler waitingAPH;
    public AnimationPointHandler startAPH;
    public bool isIn = false;
    public bool isAPHEnd = false;
    public Action<IConversationEntrySequence> AlertAPHDone { set; get; }
    public Action<IConversationEntrySequence, ConversationEntry.SuddenEndedState> AlertSuddenEnded { set; get; }
}
