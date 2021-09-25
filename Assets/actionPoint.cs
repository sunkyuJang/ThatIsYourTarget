using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ActionPoint : MonoBehaviour
{
    public enum StateKind { non = 0, idle, lookAround, sitting }
    public StateKind state = 0;
    protected int beforeState = -1;
    public float during = 0;
    [Range(1, 4)]
    public int SittingNum = 0;

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

[CustomEditor(typeof(ActionPoint)), CanEditMultipleObjects]
public class ActionPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var ap = (ActionPoint)target;
        ap.state = (ActionPoint.StateKind)EditorGUILayout.EnumPopup("State", ap.state);

        switch (ap.state)
        {
            case ActionPoint.StateKind.sitting:
                {
                    SetSittingInspector(ap);
                    break;
                }
            case ActionPoint.StateKind.idle:
                {
                    SetWaitingInspector(ap);
                    break;
                }
            case ActionPoint.StateKind.lookAround:
                {
                    SetWaitingInspector(ap);
                    break;
                }

        }
    }

    void SetSittingInspector(ActionPoint ap)
    {
        ExpresseDuring(ap);
        ap.SittingNum = (int)EditorGUILayout.Slider("SittingLevel", ap.SittingNum, 1, 4);
    }

    void SetWaitingInspector(ActionPoint ap)
    {
        ExpresseDuring(ap);
    }
    void SetLookAroundInspector(ActionPoint ap)
    {
        ExpresseDuring(ap);
    }

    void ExpresseDuring(ActionPoint ap)
    {
        ap.during = (float)EditorGUILayout.FloatField("during", ap.during);
    }
}

