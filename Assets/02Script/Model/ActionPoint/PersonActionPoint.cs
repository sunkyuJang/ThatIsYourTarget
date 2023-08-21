using UnityEditor;
using UnityEditor.Animations;

public class PersonActionPoint : AnimationPoint
{
    public PersonAniState.StateKind State { set { base.state = (int)value; } get { return (PersonAniState.StateKind)state; } }
    public override bool HasAction { get { return state != (int)PersonAniState.StateKind.Non; } }
    public bool shouldReadyForBattle;
    public int subState_int = 0;
    public bool subState_bool = false;
    public float subState_float = 0f;
    public float GetLength() => GetLength(((PersonAniState.StateKind)state).ToString());
    public PersonWeapon Weapon { get; set; }
}

[CustomEditor(typeof(PersonActionPoint))]
public class PersonActionPointEditor : Editor
{
    PersonActionPoint ap;
    public PersonAniState.StateKind kind;
    private void OnEnable()
    {
        ap = target as PersonActionPoint;
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
            case PersonAniState.StateKind.Standing:
                SetWaitingInspector(ap);
                break;
            // case PersonAniState.StateKind.PrepareAttack:
            //     SetPrepareAttack(ap);
            //     break;
            case PersonAniState.StateKind.LookAround:
                SetLookAround(ap);
                break;
            case PersonAniState.StateKind.Surprize:
                SetSurprize(ap);
                break;
            default:
                break;
        }

        ap.State = kind;
        ap.animatorController = animatorController;
        EditorUtility.SetDirty(ap);
    }
    void SetPrepareAttack(PersonActionPoint ap)
    {
        //ap.shouldReadyForBattle = EditorGUILayout.Toggle("shouldPrepare", ap);
        //if (ap.shouldReadyForBattle)
        //{
        //    ap.Weapon.weaponType = (int)EditorGUILayout.Slider("WeaponLayer", ap.weaponLayer, 1, 3);
        //}
    }

    void SetSittingInspector(PersonActionPoint ap)
    {
        ExpresseDuring(ap);
        ap.subState_int = (int)EditorGUILayout.Slider(Sitting_PersonAniState.SittingLevel, ap.subState_int, (int)Sitting_PersonAniState.SittingState.Ground, (int)Sitting_PersonAniState.SittingState.High);
    }

    void SetLookAround(PersonActionPoint ap)
    {
        ExpresseFixedDuring(ap, ap.GetLength());
    }
    void SetWaitingInspector(AnimationPoint ap) => ExpresseDuring(ap);
    void SetPrepareAttack(AnimationPoint ap) => ExpresseDuring(ap);
    void SetSurprize(PersonActionPoint ap) => ExpresseFixedDuring(ap, ap.GetLength());
    void ExpresseDuring(AnimationPoint ap) => ap.during = (float)EditorGUILayout.FloatField("during", ap.during);
    void ExpresseFixedDuring(AnimationPoint ap, float fixedTime) => ap.during = (float)EditorGUILayout.DelayedFloatField("FixedDuring", fixedTime);
}