using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ConversationEntryData : MonoBehaviour
{
    public PhysicalModelConnector physicalModelConnector;
    public IConversationSequence conversationEntrySequence
    {
        get
        {
            if (physicalModelConnector == null) return null;
            return physicalModelConnector.ConversationSequence;
        }
    }
    public AnimationPointHandler waitingAPH;
    public AnimationPointHandler startAPH;
    public bool isIn = false;
    public bool isAPHEnd = false;
    public Action<IConversationSequence> AlertAPHDone { set; get; }
    public Action<IConversationSequence, ConversationEntry.SuddenEndedState> AlertSuddenEnded { set; get; }
}
