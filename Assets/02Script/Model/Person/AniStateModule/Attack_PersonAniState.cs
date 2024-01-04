using UnityEditor.Animations;
using UnityEngine;
public class Attack_PersonAniState : PersonAniState
{
    public static string Attack { get { return "Attack"; } }
    public AttackingAnimationStateManager attackingAnimationStateManager { set; get; }
    public PersonAttackConditionHandler attackConditionerHandler { set; get; }
    public Attack_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {
        attackingAnimationStateManager = AnimatorStateManager.Instance.GetAttackingStateManager(Animator.runtimeAnimatorController as AnimatorController);
        attackConditionerHandler = new PersonAttackConditionHandler(Animator.runtimeAnimatorController as AnimatorController);
    }
    protected override void StartModule()
    {
        Animator.SetBool(UsingWeapon_PersonAniState.UsingWeapon, false);
        Animator.SetBool(Attack, true);
        var node = attackingAnimationStateManager.GetStateCopyNode(ap.animationPointData.SkillData.keyName);
        var path = attackConditionerHandler.GetAllTransitionPath(node.nowAnimation);
        path?.Invoke(Animator);
        ap.animationPointData.whenAnimationExitTime += () => { WhenAniExit(ap); };
        var stateInfo = AnimatorStateManager.Instance.GetStateInfo(Animator.runtimeAnimatorController.name, node.nowAnimation);
        ap.animationPointData.during = stateInfo.Length;
        Debug.Log(ap.animationPointData.state + "// isin _ attackStartState");
    }

    public override void EnterToException()
    {

    }

    protected void WhenAniExit(AnimationPoint ap)
    {

    }

    public override void Exit()
    {
        Debug.Log(ap.animationPointData.state + "// isin _ attackStartEnd");
        Animator.SetBool(Attack, false);
        Animator.SetBool(UsingWeapon_PersonAniState.UsingWeapon, true);
    }
}