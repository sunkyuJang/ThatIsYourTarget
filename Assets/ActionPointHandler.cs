using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointHandler : MonoBehaviour
{
    List<ActionPoint> actionPoints = new List<ActionPoint>();
    public int GetActionCount { get { return actionPoints.Count; } }
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
    }

    public ActionPoint GetActionPoint(int index)
    {
        return actionPoints[index];
    }
}
