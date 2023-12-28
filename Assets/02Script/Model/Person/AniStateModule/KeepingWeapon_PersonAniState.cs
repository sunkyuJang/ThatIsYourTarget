using UnityEngine;
public class KeepingWeapon_PersonAniState : PersonAniState
{
    public string KeepingWeapon { get { return "KeepingWeapon"; } }
    public KeepingWeapon_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }
    protected override void StartModule()
    {
        Animator.SetBool(HoldingWeapon_PersonAniState.HoldingWeapon, false);
        Animator.SetBool(UsingWeapon_PersonAniState.UsingWeapon, false);

        var layer = Animator.GetLayerIndex("WeaponMotion");
        Animator.SetLayerWeight(layer, 1);
        Animator.SetBool(KeepingWeapon, false);
        Animator.SetInteger("WeaponType", (int)ap.Weapon.GetWeaponType);
        ap.whenAnimationEnd += () => { };
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