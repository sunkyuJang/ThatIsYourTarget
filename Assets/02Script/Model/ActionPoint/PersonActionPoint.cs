using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PersonActionPoint : ActionPoint
{
    public enum StateKind { non = 0, Standing, LookAround, Sitting, Surprize, PrepareAttack, Fight, Avoid, TurnAround }
    public override bool HasAction { get { return state != (int)StateKind.non; } }
    public int sittingNum = 0;
    public bool shouldReadyForBattle;
    public bool shouldTurnLeft;
    public int weaponLayer;
}

[CustomEditor(typeof(PersonActionPoint))]
public class PersonActionPointEditor : Editor
{
    PersonActionPoint ap;
    public PersonActionPoint.StateKind kind;
    private void OnEnable()
    {
        ap = target as PersonActionPoint;
    }
    public override void OnInspectorGUI()
    {
        kind = (PersonActionPoint.StateKind)EditorGUILayout.EnumPopup("State", (PersonActionPoint.StateKind)ap.state);

        switch (kind)
        {
            case PersonActionPoint.StateKind.Sitting:
                SetSittingInspector(ap);
                break;
            case PersonActionPoint.StateKind.Standing:
                SetWaitingInspector(ap);
                break;
            case PersonActionPoint.StateKind.PrepareAttack:
                SetPrepareAttack(ap);
                break;
            default:
                break;
        }

        ap.state = (int)kind;
        EditorUtility.SetDirty(ap);
    }
    void SetPrepareAttack(PersonActionPoint ap)
    {
        ap.shouldReadyForBattle = EditorGUILayout.Toggle("shouldPrepare", ap);
        if (ap.shouldReadyForBattle)
        {
            ap.weaponLayer = (int)EditorGUILayout.Slider("WeaponLayer", ap.weaponLayer, 1, 3);
        }
    }

    void SetSittingInspector(PersonActionPoint ap)
    {
        ExpresseDuring(ap);
        ap.sittingNum = (int)EditorGUILayout.Slider("SittingLevel", ap.sittingNum, 1, 4);
    }

    void SetWaitingInspector(ActionPoint ap) => ExpresseDuring(ap);
    void SetFixedWaitingInspector(ActionPoint ap, float fixedTime) => ExpresseFixedDuring(ap, fixedTime);
    void SetPrepareAttack(ActionPoint ap) => ExpresseDuring(ap);
    void ExpresseDuring(ActionPoint ap) => ap.during = (float)EditorGUILayout.FloatField("during", ap.during);
    void ExpresseFixedDuring(ActionPoint ap, float fixedTime) => ap.during = (float)EditorGUILayout.FloatField("FixedDuring", fixedTime);
}