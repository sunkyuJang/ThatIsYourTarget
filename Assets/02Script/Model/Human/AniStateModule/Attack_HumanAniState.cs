using UnityEditor.Animations;
using UnityEngine;
public class Attack_HumanAniState : HumanAniState
{
    public static string Attack { get { return "Attack"; } }
    public AttackingAnimationStateManager attackingAnimationStateManager { set; get; }
    public HumanAttackConditionHandler attackConditionerHandler { set; get; }
    public Attack_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {
        attackingAnimationStateManager = AnimatorStateManager.Instance.GetAttackingStateManager(Animator.runtimeAnimatorController as AnimatorController);
        attackConditionerHandler = new HumanAttackConditionHandler(Animator.runtimeAnimatorController as AnimatorController);
    }
    protected override void StartModule()
    {
        Animator.SetBool(UsingWeapon_HumanAniState.UsingWeapon, false);
        Animator.SetBool(Attack, true);
        var node = attackingAnimationStateManager.GetStateCopyNode(ap.animationPointData.SkillData.keyName);
        var path = attackConditionerHandler.GetAllTransitionPath(node.nowAnimation);
        path?.Invoke(Animator);
        ap.animationPointData.whenAnimationExitTime += () => { WhenAniExit(ap); };
        var stateInfo = AnimatorStateManager.Instance.GetStateInfo(Animator.runtimeAnimatorController.name, node.nowAnimation);
        ap.animationPointData.during = stateInfo.Length;
    }

    public override void EnterToException()
    {

    }

    protected void WhenAniExit(AnimationPoint ap)
    {

    }

    public override void Exit()
    {
        Animator.SetBool(Attack, false);
    }
}