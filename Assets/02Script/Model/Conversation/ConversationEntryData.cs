using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ConversationEntryData
{
    public Collider targetCollider;
    public IConversationEntrySequence conversationEntrySequence;
    public AnimationPointHandler waitingAph;
    public AnimationPointHandler startAph;
    public bool isIn = false;
    public bool isAPHEnd = false;
}
