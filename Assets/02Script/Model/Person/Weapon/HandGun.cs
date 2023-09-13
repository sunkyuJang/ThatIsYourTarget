using UnityEngine;

public class HandGun : PersonWeapon
{
    new private void Awake()
    {
        base.Awake();
        weaponType = WeaponType.HandGun;
    }

    public override void AttachToHand(Animator HumanoidAnimator)
    {
        //base.SetHandIKAnimation(HumanoidAnimator);
        //var leftHand = HumanoidAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
        //HumanBodyBones.
    }
}
