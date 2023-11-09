using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationJobManager : JobManager
{
    public enum State { Prepare, Start, End, SuddenEnded, Non }
    public ConversationJobManager(object section, Action runAfterJobEnd) : base(section, runAfterJobEnd)
    {

    }

    protected List<Job> MakeJobList()
    {
        return null;
    }

    protected void PrepareConversation()
    {

    }
}
