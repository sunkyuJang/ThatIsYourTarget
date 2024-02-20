using UnityEngine;
public class KeepingWeapon_HumanAniState : HumanAniState
{
    public string KeepingWeapon { get { return "KeepingWeapon"; } }
    public KeepingWeapon_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }
    protected override void StartModule()
    {
        Animator.SetBool(HoldingWeapon_HumanAniState.HoldingWeapon, false);
        Animator.SetBool(UsingWeapon_HumanAniState.UsingWeapon, false);

        var layer = Animator.GetLayerIndex("WeaponMotion");
        Animator.SetLayerWeight(layer, 1);
        Animator.SetBool(KeepingWeapon, false);
        Animator.SetInteger("WeaponType", (int)ap.animationPointData.Weapon.GetWeaponType);
        ap.animationPointData.whenAnimationEnd += () => { };
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        var layer = Animator.GetLayerIndex("WeaponMotion");
        Animator.SetLayerWeight(layer, 0);
        Animator.SetBool(KeepingWeapon, false);
    }
}