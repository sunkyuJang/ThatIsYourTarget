using UnityEditor;
using UnityEngine;

public class HumanWeapon : Weapon
{
    /// <summary>
    /// u shouldnt change this info in runtime. 
    /// </summary>
    public HumanFingerPositioner humanFingerPositioner;
    new protected void Awake()
    {
        base.Awake();
    }

    public virtual void AttachToHand(Animator HumanoidAnimator) { }
}