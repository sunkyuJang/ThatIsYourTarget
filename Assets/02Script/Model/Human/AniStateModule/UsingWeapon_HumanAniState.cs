using UnityEngine;
public class UsingWeapon_HumanAniState : HumanAniState
{
    public static string UsingWeapon { get { return "UsingWeapon"; } }
    private float minTransitionTime = 0.3f;
    public UsingWeapon_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }

    public override bool IsReady()
    {
        return base.IsReady();
    }
    protected override void StartModule()
    {
        Animator.SetBool(HoldingWeapon_HumanAniState.HoldingWeapon, false);

        var layer = Animator.GetLayerIndex("WeaponMotion");
        Animator.SetLayerWeight(layer, 1);
        Animator.SetBool(UsingWeapon, true);
        Animator.SetInteger("WeaponType", (int)ap.animationPointData.Weapon.GetWeaponType);

        ap.animationPointData.during = ap.animationPointData.during < minTransitionTime ? minTransitionTime : ap.animationPointData.during;
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }

}