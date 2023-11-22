using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

[RequireComponent(typeof(ObjDetector))]
public class ConversationEntry : MonoBehaviour, IObjDetectorConnector_OnDetected
{
    public enum SuddenEndedState { Hold, UnHold, Combat, No_Response, Non }
    private List<ConversationEntryData> conversationEntryDatas = new List<ConversationEntryData>();

    private void Awake()
    {
        conversationEntryDatas = transform.GetComponentsInChildren<ConversationEntryData>().ToList();
        ResetConversationData();
    }
    private void ResetConversationData()
    {
        conversationEntryDatas.ForEach(x =>
        {
            x.isIn = false;
            x.isAPHEnd = false;
        });
    }
    public void OnDetected(ObjDetector detector, Collider collider)
    {
        var physicalModelConnector = collider.GetComponent<PhysicalModelConnector>();
        if (physicalModelConnector == null) return;

        var find = conversationEntryDatas.Find(x => x.physicalModelConnector == physicalModelConnector);
        if (find == null) return;

        if (find.conversationEntrySequence.CanEnterConversation())
        {
            find.conversationEntrySequence.PrepaerConversation(find.waitingAPH, find.startAPH, AlertEndedAPH, AlertSuddenEnded);
            find.isIn = true;

            var isInCount = 0;
            for (int i = 0; i < conversationEntryDatas.Count; i++)
            {
                var data = conversationEntryDatas[i];
                if (data.isIn)
                    isInCount++;
            }

            if (isInCount == conversationEntryDatas.Count)
            {
                StartConversation();
            }
        }
    }

    private void StartConversation()
    {
        conversationEntryDatas.ForEach(x =>
        {
            x.conversationEntrySequence.StartConversation();
        });
    }

    private void EndConversation()
    {
        conversationEntryDatas.ForEach(x => x.conversationEntrySequence.EndConversation());
    }

    private void AlertEndedAPH(IConversationSequence target)
    {
        var data = conversationEntryDatas.Find(x => x.conversationEntrySequence == target);
        var count = 0;
        conversationEntryDatas.ForEach(x =>
        {
            if (x == data) x.isAPHEnd = true;

            if (x.isAPHEnd) count++;
        });

        if (count == conversationEntryDatas.Count)
            EndConversation();
    }

    private void AlertSuddenEnded(IConversationSequence requestedTarget, SuddenEndedState suddenEndedState)
    {
        var find = conversationEntryDatas.Find(x => x.conversationEntrySequence == requestedTarget);
        conversationEntryDatas.ForEach(x =>
        {
            if (x.conversationEntrySequence != requestedTarget)
                if (x.isIn)
                {
                    var eachTarget = x.conversationEntrySequence;
                    switch (suddenEndedState)
                    {
                        case SuddenEndedState.Hold: eachTarget.AlertHold(); break;
                        case SuddenEndedState.UnHold: eachTarget.AlertUnHold(); break;
                        case SuddenEndedState.Combat: eachTarget.AlertCombat(find.physicalModelConnector.transform.position); break;
                        case SuddenEndedState.No_Response: eachTarget.AlertNonResponce(find.physicalModelConnector.transform.position); break;
                        case SuddenEndedState.Non: break;
                    }
                }
        });
    }
}
