using UnityEngine;
public class UsingWeapon_PersonAniState : PersonAniState
{
    public static string UsingWeapon { get { return "UsingWeapon"; } }
    private float minTransitionTime = 0.3f;
    public UsingWeapon_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {

    }

    public override bool IsReady()
    {
        Debug.Log(base.IsReady());
        return base.IsReady();
    }
    protected override void StartModule()
    {
        Animator.SetBool(HoldingWeapon_PersonAniState.HoldingWeapon, false);

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