using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointHandler : MonoBehaviour
{
    public List<ActionPoint> actionPoints { private set; get; } = new List<ActionPoint>();
    List<ActionPoint> originalAPs { set; get; } = new List<ActionPoint>();
    public int GetActionCount { get { return transform.childCount; } }
    public int index = 0;
    public bool ShouldLoop = true;
    public bool IsReachedToEnd = false;
    Coroutine processingMemorizeStateUntillIsReachedEnd;
    public System.Action<ActionPointHandler> comingFromOther = null;
    public void Awake()
    {
        SetAPs();
    }

    public void SetAPs()
    {
        SetActionPoint();
    }

    void SetActionPoint()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            actionPoints.Add(transform.GetChild(i).GetComponent<ActionPoint>());
        }
        originalAPs = actionPoints;
        ResetIndex();
    }

    void CheckLastAP()
    {
        if (!ShouldLoop)
        {
            if (actionPoints.Count > 1)
            {
                var firstAP = actionPoints[0];
                var lastAP = actionPoints[actionPoints.Count - 1];
                if (lastAP.during >= 0 && firstAP.during >= 0)
                {
                    Debug.LogError("when APH is not running with loop, first and last 'AP.during' should be lower than '0'.");
                    firstAP.during = -1;
                    lastAP.during = -1;
                }
            }
            else
            {
                Debug.LogError("when APH is not running with loop, APH should have more than 2 APs as for start and end.");
            }
        }
    }

    public ActionPoint GetNextActionPoint()
    {
        if (ShouldLoop)
            index %= GetActionCount;
        else
        {
            if (index >= GetActionCount - 1)
            {
                index = GetActionCount - 1;
                IsReachedToEnd = true;
            }
        }

        return GetActionPoint(index++);
    }

    public ActionPoint GetActionPoint(int index)
    {
        var ap = actionPoints[index];
        return ap;
    }

    public ActionPoint GetLastActionPoint
    {
        get { return actionPoints[actionPoints.Count - 1]; }
    }
    public void WaitForStartToNext(int index, bool ShouldWait)
    {
        if (ShouldWait)
            actionPoints[index].BackUpTime(-1);
        else
            actionPoints[index].RecoverTime();
    }

    public void ResetIndex() => index = 0;

    public void ChangeAPPositionAndLookAt(int index, Vector3 from, Vector3 to)
    {
        actionPoints[index].ChangePosition(to);
        actionPoints[index].MakeLookAtTo(from, to);
    }
}
