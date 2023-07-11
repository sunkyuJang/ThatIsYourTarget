using UnityEngine;
using UnityEditor.Animations;

internal class PrepareAttack_PersonAniState : PersonAniState
{
    public enum AttackLayer { Pist, HandGun, AR, Reset, Non }
    protected AttackLayer attackLayer = AttackLayer.Non;
    public PrepareAttack_PersonAniState(Animator ani) : base(ani) { }
    public void SetAttackLayer(AttackLayer attackLayer) => this.attackLayer = attackLayer;
    public override bool IsReadyForEnter()
    {
        return base.IsReadyForEnter() && attackLayer != AttackLayer.Non;
    }
    protected override void DoEnter()
    {
        var layer = 0;
        switch (attackLayer)
        {
            case AttackLayer.Pist: layer = Animator.GetLayerIndex("HoldingHandGun"); break;
            case AttackLayer.HandGun: layer = Animator.GetLayerIndex("HoldingHandGun"); break;
            case AttackLayer.AR: layer = Animator.GetLayerIndex("HoldingAR"); break;
        }

        Animator.SetLayerWeight(layer, 1);
    }

    void SetPrepareAttackLayer(AttackLayer attackLayer)
    {
        RuntimeAnimatorController runtimeController = Animator.runtimeAnimatorController as AnimatorController;
        //var layerCount = runtimeController.
    }

    public override void EnterToException()
    {
    }

    public override void Exit()
    {

    }
}