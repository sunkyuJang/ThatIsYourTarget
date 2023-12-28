using UnityEngine;
public class Attack_PersonAniState : PersonAniState
{
    public string Attack { get { return "Attack"; } }
    public Attack_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {
    }
    protected override void StartModule()
    {
        Animator.SetBool(UsingWeapon_PersonAniState.UsingWeapon, false);
        Animator.SetBool(Attack, true);
        ap.AttackComboStateNode.WhenReadAPForSetParametter?.Invoke(Animator);
        ap.whenAnimationExitTime += () => { WhenAniExit(ap); };
        var stateInfo = AnimatorStateManager.Instance.GetStateInfo(Animator.runtimeAnimatorController.name, ap.AttackComboStateNode.nowAnimation);
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