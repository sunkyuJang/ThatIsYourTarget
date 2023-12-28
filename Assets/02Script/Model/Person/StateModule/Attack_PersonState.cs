using System;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Attack_PersonState : PersonState
{
    PersonAttackConditionHandler AttackConditioner { set; get; }
    public Attack_PersonState(Person person) : base(person)
    {
        AttackConditioner = new PersonAttackConditionHandler(ActorTransform.GetComponent<Animator>().runtimeAnimatorController as AnimatorController);
    }
    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        if (Weapon == null)
        {
            // cant attack
            SetNormalState();
        }

        if (Weapon.CanAttack(out Weapon.CanAttackStateError attackError))
        {
            SetAttack(null);
        }
        else if (attackError == global::Weapon.CanAttackStateError.OverMaxCount)
        {
            // reloading
        }
    }

    void SetAttack(AnimationComboStateNode loopNode)
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
        var AttackAP = aph.GetAnimationPoint<PersonAnimationPoint>(0);
        SetAPs(AttackAP, prepareData.target, PersonAniState.StateKind.Attack, 0, false, true);
        AttackAP.EventTrigger = AttackTrigger; // for actual attack timing
        var ConditionRequireData = new PersonAttackConditionHandler.PersonRequireData(Weapon);
        var node = loopNode == null ? AttackConditioner.GetInitiatedNode(ConditionRequireData) : loopNode; // if its loop, keep using old one
        node.loopCount++;
        AttackAP.AttackComboStateNode = node;
        AttackAP.whenAnimationExitTime = () => { WhenExitTime(AttackAP); }; // every exit time, the attack state will issue an AP
        SetAPH(aph, true);
    }

    public void AttackTrigger(int num)
    {
        Debug.Log("isIn");
    }

    public void WhenExitTime(AnimationPoint ap)
    {
        // checking loop
        if (ap.Weapon == Weapon)
        {
            var node = ap.AttackComboStateNode;
            if (node.maxLoop != 0)
            {
                if (node.loopCount <= node.maxLoop)
                {
                    SetAttack(node);
                    return;
                }
            }
        }
        StartModule();
    }


    protected override void AfterAPHDone()
    {
        if (IsInSight(prepareData.target))
            StartModule();
        else
            SetState(StateKinds.Tracking, prepareData);
    }
    public override void Exit()
    {
        base.Exit();
    }
}
