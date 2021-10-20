using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointHandler : MonoBehaviour
{
    List<ActionPoint> actionPoints = new List<ActionPoint>();
    List<ActionPoint> originalAPs = new List<ActionPoint>();
    public int GetActionCount { get { return actionPoints.Count; } }
    public int index = 0;
    public bool ShouldLoop = true;
    public bool IsReachedToEnd = false;
    Coroutine processingMemorizeStateUntillIsReachedEnd;
    private void Awake()
    {
        SetActionPoint();
        CheckLastAP();
    }

    void SetActionPoint()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            actionPoints.Add(transform.GetChild(i).GetComponent<ActionPoint>());
        }
        originalAPs = actionPoints;
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
    public void WaitForStartToNext(bool ShouldWait)
    {
        actionPoints[0].during = ShouldWait ? -1 : 0;
    }

    public void ResetIndex() => index = 0;
}
