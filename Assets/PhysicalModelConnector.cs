using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JExtentioner;

public class PhysicalModelConnector : MonoBehaviour
{
    private Model Model { set; get; }
    public IConversationEntrySequence ConversationEntrySequence { private set; get; }
    private void Awake()
    {
        var modelTransform = transform.FindStartingParentWithSameTag();
        Model = modelTransform.GetComponent<Model>();
    }
}
