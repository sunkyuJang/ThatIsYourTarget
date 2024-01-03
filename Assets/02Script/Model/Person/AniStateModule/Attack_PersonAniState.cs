using UnityEditor.Animations;
using UnityEngine;
public class Attack_PersonAniState : PersonAniState
{
    public string Attack { get { return "Attack"; } }
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
        var node = attackingAnimationStateManager.GetStateCopyNode(ap.SkillData.keyName);
        var path = attackConditionerHandler.GetAllTransitionPath(node.nowAnimation);
        path?.Invoke(Animator);
        ap.whenAnimationExitTime += () => { WhenAniExit(ap); };
        var stateInfo = AnimatorStateManager.Instance.GetStateInfo(Animator.runtimeAnimatorController.name, node.nowAnimation);
        ap.during = stateInfo.Length;
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
        Animator.SetBool(UsingWeapon_PersonAniState.UsingWeapon, true);
    }

}