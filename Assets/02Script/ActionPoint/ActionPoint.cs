using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ActionPoint : MonoBehaviour
{
    public virtual bool HasNoAction { get { return true; } }

    public int state = -1;

    public float during = 0;
    float originalDuring = 0;
    // public bool IsDoing
    // {
    //     get { return }
    // public void StartTimeCount()
    // {
    //     if (!IsDoing)
    //         StartCoroutine(DoTimeCount(null));
    // }
    // public void StartTimeCount(Action action)
    // {
    //     if (!IsDoing)
    //         StartCoroutine(DoTimeCount(action));
    // }

    // public void BackUpTime(float insteadTime)
    // {
    //     originalDuring = during;
    //     during = insteadTime;
    // }

    // public void RecoverTime() => during = originalDuring;

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
    public void MakeLookAtTo(Vector3 from, Vector3 to) => transform.LookAt(Vector3.Normalize(from - to));
}

