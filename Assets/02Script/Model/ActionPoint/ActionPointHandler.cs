using System.Collections.Generic;
using UnityEngine;

public class ActionPointHandler : MonoBehaviour
{
    public enum WalkingState { Non, Walk, Run }
    public WalkingState walkingState = WalkingState.Walk;
    public List<ActionPoint> actionPoints { set; get; } = new List<ActionPoint>();
    public int GetActionCount { get { return actionPoints.Count; } }
    public int index = 0;
    public bool shouldLoop = true;
    public bool isAPHDone { get { return !shouldLoop && index >= GetActionCount; } }
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

    public ActionPoint GetNowActionPoint() { return isAPHDone ? null : actionPoints[index]; }

    public ActionPoint GetNextActionPoint()
    {
        ++index;
        if (shouldLoop)
            index %= GetActionCount;
        else if (isAPHDone)
            return null;

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
