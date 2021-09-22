using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actionPoint : MonoBehaviour
{
    public enum StateKind { waiting = 0, lookAround, sitting, non }
    public StateKind state = 0;
    protected int beforeState = -1;
    public float during = 0;

    public bool IsDoing { protected set; get; } = false;
    public void StartTimeCount()
    {
        if (!IsDoing)
            StartCoroutine(DoTimeCount());
    }
    IEnumerator DoTimeCount()
    {
        IsDoing = true;
        var t = 0f;
        var maxT = during;
        while (t < maxT)
        {
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        IsDoing = false;
    }
}
