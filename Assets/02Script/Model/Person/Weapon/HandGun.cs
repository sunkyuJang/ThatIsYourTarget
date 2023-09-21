using UnityEngine;

public class HandGun : PersonWeapon
{
    new private void Awake()
    {
        base.Awake();
        weaponType = WeaponType.HandGun;
        Range = 5f;
        Dmg = 1;
    }

    public override void AttachToHand(Animator HumanoidAnimator)
    {
        //base.SetHandIKAnimation(HumanoidAnimator);
        //var leftHand = HumanoidAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
        //HumanBodyBones.
    }
}
