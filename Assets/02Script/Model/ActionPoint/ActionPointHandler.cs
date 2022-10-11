using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointHandler : MonoBehaviour
{
    public GameObject originalOwener;
    public List<ActionPoint> actionPoints { set; get; } = new List<ActionPoint>();
    public int GetActionCount { get { return actionPoints.Count; } }
    public int index = 0;
    public bool ShouldLoop = true;
    public bool IsReachedToEnd = false;
    public void Awake()
    {
        SetAPs();
    }

    public void SetAPs(List<ActionPoint> list = null)
    {
        if (list != null)
        {
            list.ForEach(x => x.transform.SetParent(transform));
        }
        SetActionPoint();
    }

    void SetActionPoint()
    {
        actionPoints.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            actionPoints.Add(transform.GetChild(i).GetComponent<ActionPoint>());
        }
        ResetIndex();
    }

    public ActionPoint GetNowActionPoint() { return actionPoints[index]; }

    public ActionPoint GetNextActionPoint()
    {
        ++index;
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

        return GetActionPoint(index);
    }

    public ActionPoint GetActionPoint(int index)
    {
        var ap = actionPoints[index];
        return ap;
    }

    public ActionPoint GetEndActionPoint
    {
        get { return actionPoints[actionPoints.Count - 1]; }
    }

    public void ResetIndex() => index = 0;

    public void ChangeAPPositionAndLookAt(int index, Vector3 from, Vector3 to)
    {
        actionPoints[index].ChangePosition(to);
        actionPoints[index].MakeLookAtTo(to);
    }
}
