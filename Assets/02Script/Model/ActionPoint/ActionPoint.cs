using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class ActionPoint : MonoBehaviour
{
    public virtual bool HasAction { get { return true; } }
    [HideInInspector]
    public int state = 0;
    [HideInInspector]
    public float during = 0;
    [HideInInspector]
    float originalDuring = 0;

    IEnumerator DoTimeCount(Action action)
    {
        var t = 0f;
        var maxT = during;
        if (during >= 0)
        {
            while (t < maxT)
            {
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (during < 0)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        action?.Invoke();
    }
    public void ChangePosition(Vector3 position) => transform.position = position;
    public void MakeLookAtTo(Vector3 to) => transform.LookAt(to - Vector3.up * to.y);
    public void SetPositionForTracking(Transform from, Transform to, bool shouldChangeRotation, bool shouldChangePosition = false)
    {
        ChangePosition(from.position);
        MakeLookAtTo(shouldChangeRotation ? to.position : from.forward);
        if (shouldChangePosition)
            ChangePosition(to.position);
    }

    public void SetAPWithDuring(Transform from, Transform to, PersonActionPoint.StateKind kind, float time, bool shouldChangeRotation = false, bool shouldChangePosition = false)
    {
        state = (int)kind;
        during = time;
        SetPositionForTracking(from, to, shouldChangeRotation, shouldChangePosition);
    }
}

