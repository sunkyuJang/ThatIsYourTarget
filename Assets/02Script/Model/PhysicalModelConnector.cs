using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JExtentioner;

public class PhysicalModelConnector : MonoBehaviour
{
    public Model Model { private set; get; }
    public IConversationSequence ConversationSequence { private set; get; }
    bool isSet = false;
    public void SetPhysicalModelConnector(Model model, IConversationSequence conversationSequence)
    {
        Model = model;
        ConversationSequence = conversationSequence;
    }
}