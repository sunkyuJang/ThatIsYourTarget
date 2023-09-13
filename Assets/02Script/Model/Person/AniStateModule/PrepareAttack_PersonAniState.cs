using UnityEngine;

internal class PrepareAttack_PersonAniState : PersonAniState
{
    public PrepareAttack_PersonAniState(Animator ani) : base(ani) { }
    public override bool IsReady()
    {
        return base.IsReady();
    }
    protected override void StartModule()
    {
        var layer = 0;
        switch (ap.Weapon.weaponType)
        {
            case PersonWeapon.WeaponType.Non: layer = Animator.GetLayerIndex("Non"); break;
            case PersonWeapon.WeaponType.HandGun: layer = Animator.GetLayerIndex("HoldingHandGun"); break;
            case PersonWeapon.WeaponType.AR: layer = Animator.GetLayerIndex("HoldingAR"); break;
        }

        if (ap.Weapon == null)
        {

        }
        else
        {
            Animator.SetLayerWeight(layer, 1);
        }
    }

    void SetHandsIK()
    {
        for (int i = 0; i < 2; i++)
        {
            var hands = i == 0 ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
            var position = i == 0 ? ap.Weapon.LGrab : ap.Weapon.RGrab;

            Animator.SetIKPositionWeight(hands, 1);
            Animator.SetIKPosition(hands, position.transform.position);
        }
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }
}