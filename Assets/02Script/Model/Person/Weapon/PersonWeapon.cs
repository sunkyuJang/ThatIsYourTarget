using UnityEditor;
using UnityEngine;

public class PersonWeapon : Weapon
{
    public Transform AimTransform;

    public void Awake()
    {
    }

    public virtual void AttachToHand(Animator HumanoidAnimator) { }
}