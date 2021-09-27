using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointHandler : MonoBehaviour
{
    List<ActionPoint> actionPoints = new List<ActionPoint>();
    List<ActionPoint> originalAPs = new List<ActionPoint>();
    public int GetActionCount { get { return actionPoints.Count; } }
    public int index = 0;
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
        index %= GetActionCount;
        return GetActionPoint(index++);
    }

    public ActionPoint GetActionPoint(int index)
    {
        var ap = actionPoints[index];
        return ap;
    }
}
