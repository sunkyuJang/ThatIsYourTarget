using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointHandler : MonoBehaviour
{
    List<actionPoint> actionPoints = new List<actionPoint>();
    public int GetActionCount { get { return actionPoints.Count; } }
    private void Awake()
    {
        SetActionPoint();
    }

    void SetActionPoint()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            actionPoints.Add(transform.GetChild(i).GetComponent<actionPoint>());
        }
    }

    public actionPoint GetActionPoint(int index)
    {
        return actionPoints[index];
    }
}
