using JExtentioner;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
public class PersonAnimationPoint : AnimationPoint
{
    public PersonAniState.StateKind State
    {
        get { return (PersonAniState.StateKind)base.state; }
    }

    public override bool ShouldPlaySamePosition { get => PersonAniState.shouldPlaySamePositions.Contains(State); }
    public override void ReplaceExpectionState() => state = (int)PersonAniState.replacebleState;
    public override bool HasAction { get { return base.state != (int)PersonAniState.StateKind.Non; } }
    public bool shouldReadyForBattle;
    public int subState_int = 0;
    public bool subState_bool = false;
    public float subState_float = 0f;
    public float GetLength() => GetAnimationClipLength(((PersonAniState.StateKind)base.state).ToString());
    new public PersonWeapon Weapon { get { return base.Weapon as PersonWeapon; } }
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

    // public override bool IsFixedDuringInRuntime(int state)
    // {
    //     return PersonAniState.IsStateDuringFixedAfterRuntime((PersonAniState.StateKind)state);
    // }

    public override string GetStateName(int state)
    {
        return state.GetEnumVal<PersonAniState.StateKind>()?.ToString();
    }

    public override string GetRuntimeStateName(int state)
    {
        var aniState = (PersonAniState.StateKind)state;
        // if (PersonAniState.IsWeaponRuntimeState(aniState))
        // {
        //     return Weapon.weaponType.ToString() + aniState.ToString();
        // }
        // else 
        if (PersonAniState.IsAttackKind(aniState))
        {
            return Weapon.GetWeaponType.ToString() + aniState.ToString() + subState_int.ToString();
        }

        return "";
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

        ap.state = (int)kind;
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
        ap.during = find == ap.State ? (float)EditorGUILayout.DelayedFloatField("FixedDuring", ap.GetLength())
                                        : (float)EditorGUILayout.FloatField("during", ap.during);
    }
}