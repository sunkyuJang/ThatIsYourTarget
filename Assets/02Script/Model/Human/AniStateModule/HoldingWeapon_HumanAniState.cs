using UnityEngine;

internal class HoldingWeapon_HumanAniState : HumanAniState
{
    public static string HoldingWeapon { get { return "HoldingWeapon"; } }
    public HoldingWeapon_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler) { }
    public override bool IsReady()
    {
        return base.IsReady();
    }
    protected override void StartModule()
    {
        Animator.SetBool(UsingWeapon_HumanAniState.UsingWeapon, false);

        var layer = Animator.GetLayerIndex("WeaponMotion");
        Animator.SetLayerWeight(layer, 1);
        Animator.SetBool(HoldingWeapon, true);
        Animator.SetInteger("WeaponType", (int)ap.Weapon.GetWeaponType);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }
}