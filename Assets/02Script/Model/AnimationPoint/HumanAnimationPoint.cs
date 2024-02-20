using JExtentioner;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
public class HumanAnimationPoint : AnimationPoint, IPoolerConnector
{
    public HumanAniState.StateKind State
    {
        get { return (HumanAniState.StateKind)animationPointData.state; }
    }
    public override bool HasAction { get { return base.animationPointData.state != (int)HumanAniState.StateKind.Non; } }
    public bool shouldReadyForBattle;
    public int subState_int = 0;
    public bool subState_bool = false;
    public float subState_float = 0f;
    public float GetLength() => GetAnimationClipLength(((HumanAniState.StateKind)base.animationPointData.state).ToString());
    public HumanWeapon Weapon { get { return base.animationPointData.Weapon as HumanWeapon; } }

    public override bool ShouldPlaySamePosition => HumanAniState.shouldPlaySamePositions.Contains(State);

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
        return HumanAniState.IsStateDuringFixed((HumanAniState.StateKind)state);
    }

    public override string GetStateName(int state)
    {
        return state.GetEnumVal<HumanAniState.StateKind>()?.ToString();
    }

    public override string GetRuntimeStateName(int state)
    {
        var aniState = (HumanAniState.StateKind)state;
        if (HumanAniState.IsAttackKind(aniState))
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

[CustomEditor(typeof(HumanAnimationPoint))]
public class HumanActionPointEditor : Editor
{
    HumanAnimationPoint ap;
    public HumanAniState.StateKind kind;
    private void OnEnable()
    {
        ap = target as HumanAnimationPoint;
    }
    public override void OnInspectorGUI()
    {
        var animatorController = EditorGUILayout.ObjectField("Animator Contoller", ap.animatorController, typeof(AnimatorController), false) as AnimatorController;

        kind = (HumanAniState.StateKind)EditorGUILayout.EnumPopup("State", ap.State);

        switch (kind)
        {
            case HumanAniState.StateKind.Sitting:
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

    void SetSittingInspector(HumanAnimationPoint ap)
    {
        ap.subState_int = (int)EditorGUILayout.Slider(Sitting_HumanAniState.SittingLevel, ap.subState_int, (int)Sitting_HumanAniState.SittingState.Ground, (int)Sitting_HumanAniState.SittingState.High);
    }

    void ExpresseDuring(HumanAnimationPoint ap)
    {
        var find = HumanAniState.FixedDuringStateKinds.Find(x => x == ap.State);
        ap.animationPointData.during = find == ap.State ? (float)EditorGUILayout.DelayedFloatField("FixedDuring", ap.GetLength())
                                        : (float)EditorGUILayout.FloatField("during", ap.animationPointData.during);
    }
}