using JExtentioner;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
public class PersonAnimationPoint : AnimationPoint, IPoolerConnector
{
    public PersonAniState.StateKind State
    {
        get { return (PersonAniState.StateKind)animationPointData.state; }
    }
    public override bool HasAction { get { return base.animationPointData.state != (int)PersonAniState.StateKind.Non; } }
    public bool shouldReadyForBattle;
    public int subState_int = 0;
    public bool subState_bool = false;
    public float subState_float = 0f;
    public float GetLength() => GetAnimationClipLength(((PersonAniState.StateKind)base.animationPointData.state).ToString());
    public PersonWeapon Weapon { get { return base.animationPointData.Weapon as PersonWeapon; } }

    public override bool ShouldPlaySamePosition => PersonAniState.shouldPlaySamePositions.Contains(State);

    public List<float> GetAnimationEvent()
    {
        return GetAnimationEvent(State.ToString());
    }
    public List<KeyValuePair<float, string>> GetExitEvent(string animationName)
    {
        return GetExitAniEvent(animationName);
    }

    public override bool IsFixedDuring(int state)
    {
        return PersonAniState.IsStateDuringFixed((PersonAniState.StateKind)state);
    }

    public override string GetStateName(int state)
    {
        return state.GetEnumVal<PersonAniState.StateKind>()?.ToString();
    }

    public override string GetRuntimeStateName(int state)
    {
        var aniState = (PersonAniState.StateKind)state;
        if (PersonAniState.IsAttackKind(aniState))
        {
            return Weapon.GetWeaponType.ToString() + aniState.ToString() + subState_int.ToString();
        }

        return "";
    }

    public override void ReplaceExpectionState()
    {
        throw new System.NotImplementedException();
    }
}

[CustomEditor(typeof(PersonAnimationPoint))]
public class PersonActionPointEditor : Editor
{
    PersonAnimationPoint ap;
    public PersonAniState.StateKind kind;
    private void OnEnable()
    {
        ap = target as PersonAnimationPoint;
    }
    public override void OnInspectorGUI()
    {
        var animatorController = EditorGUILayout.ObjectField("Animator Contoller", ap.animatorController, typeof(AnimatorController), false) as AnimatorController;

        kind = (PersonAniState.StateKind)EditorGUILayout.EnumPopup("State", ap.State);

        switch (kind)
        {
            case PersonAniState.StateKind.Sitting:
                SetSittingInspector(ap);
                break;
            default:
                break;
        }
        ExpresseDuring(ap);

        ap.animationPointData.state = (int)kind;
        ap.animatorController = animatorController;
        EditorUtility.SetDirty(target);
    }

    void SetSittingInspector(PersonAnimationPoint ap)
    {
        ap.subState_int = (int)EditorGUILayout.Slider(Sitting_PersonAniState.SittingLevel, ap.subState_int, (int)Sitting_PersonAniState.SittingState.Ground, (int)Sitting_PersonAniState.SittingState.High);
    }

    void ExpresseDuring(PersonAnimationPoint ap)
    {
        var find = PersonAniState.FixedDuringStateKinds.Find(x => x == ap.State);
        ap.animationPointData.during = find == ap.State ? (float)EditorGUILayout.DelayedFloatField("FixedDuring", ap.GetLength())
                                        : (float)EditorGUILayout.FloatField("during", ap.animationPointData.during);
    }
}