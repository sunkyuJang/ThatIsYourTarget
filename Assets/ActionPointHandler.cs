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
    }

    void SetActionPoint()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            actionPoints.Add(transform.GetChild(i).GetComponent<ActionPoint>());
        }
        originalAPs = actionPoints;
    }

    public ActionPoint GetNextActionPoint()
    {
        if (ShouldLoop)
            index %= GetActionCount;
        else
            if (index >= GetActionCount)
        {
            index = GetActionCount - 1;
            IsReachedToEnd = true;
        }
        return GetActionPoint(index++);
    }

    public ActionPoint GetActionPoint(int index)
    {
        var ap = actionPoints[index];
        return ap;
    }

    public void MemorizeLastAPStateUntillIsReachedEnd()
    {
        if (processingMemorizeStateUntillIsReachedEnd == null)
            processingMemorizeStateUntillIsReachedEnd = StartCoroutine(DoMemorizeLastAPStatUntillIsReachedEnd());
    }
    IEnumerator DoMemorizeLastAPStatUntillIsReachedEnd()
    {
        var lastAP = GetNextActionPoint();
        var lastAPDuring = lastAP.during;
        lastAP.during = -1;
        yield return new WaitUntil(() => !IsReachedToEnd);
        lastAP.during = lastAPDuring;
        index = 0;

        processingMemorizeStateUntillIsReachedEnd = null;
        yield return null;
    }
}
