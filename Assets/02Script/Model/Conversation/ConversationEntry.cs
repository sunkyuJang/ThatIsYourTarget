using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class ConversationEntry : MonoBehaviour, IObjDetectorConnector_OnDetected
{
    public enum SuddenEndedState { Hold, UnHold, Combat, No_Response, Non }
    [SerializeField]
    protected List<ConversationEntryData> conversationEntryDatas = new List<ConversationEntryData>();
    public ConversationEntryData GetData(IConversationEntrySequence target) => conversationEntryDatas.Find(x => x.conversationEntrySequence == target);
    private void Awake()
    {
        if (conversationEntryDatas.Any())
        {
            conversationEntryDatas.ForEach(x =>
            {
                x.conversationEntrySequence = x.targetCollider.GetComponent<PhysicalModelConnector>().ConversationEntrySequence;
            });
        }

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
        var find = conversationEntryDatas.Find(x => x.targetCollider == collider);
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

    private void AlertEndedAPH(IConversationEntrySequence target)
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

    private void AlertSuddenEnded(IConversationEntrySequence requestedTarget, SuddenEndedState suddenEndedState)
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
                        case SuddenEndedState.Combat: eachTarget.AlertCombat(find.targetCollider.transform.position); break;
                        case SuddenEndedState.No_Response: eachTarget.AlertNonResponce(find.targetCollider.transform.position); break;
                        case SuddenEndedState.Non: break;
                    }
                }
        });
    }
}
