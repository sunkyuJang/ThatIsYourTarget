using UnityEngine;

public class HandGun : HumanWeapon
{
    new private void Awake()
    {
        base.Awake();
        range = 5f;
        dmg = 1;
    }

    public override void AttachToHand(Animator HumanoidAnimator)
    {
        //base.SetHandIKAnimation(HumanoidAnimator);
        //var leftHand = HumanoidAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
        //HumanBodyBones.
    }
}
