using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PersonActionPoint : ActionPoint
{
    public enum StateKind { non = 0, standing, lookAround, sitting }

    public override bool HasNoAction { get { return state == (int)StateKind.non; } }

    [Range(1, 4)]
    public int SittingNum = 0;
}

[CustomEditor(typeof(PersonActionPoint)), CanEditMultipleObjects]
public class ActionPointEditor : Editor
{
    public PersonActionPoint.StateKind kind = PersonActionPoint.StateKind.non;
    public override void OnInspectorGUI()
    {
        var ap = (PersonActionPoint)target;
        kind = (PersonActionPoint.StateKind)EditorGUILayout.EnumPopup("State", kind);

        switch (kind)
        {
            case PersonActionPoint.StateKind.sitting:
                {
                    SetSittingInspector(ap);
                    break;
                }
            case PersonActionPoint.StateKind.standing:
                {
                    SetWaitingInspector(ap);
                    break;
                }
            case PersonActionPoint.StateKind.lookAround:
                {
                    SetWaitingInspector(ap);
                    break;
                }
        }

        if ((int)kind != ap.state)
        {
            ap.state = (int)kind;
        }
    }

    void SetSittingInspector(PersonActionPoint ap)
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