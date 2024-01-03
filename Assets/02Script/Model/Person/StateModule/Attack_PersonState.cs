using System;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Attack_PersonState : PersonState
{
    public Attack_PersonState(Person person) : base(person)
    {

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

        if (Weapon.CanAttack(prepareData.target, out Weapon.CanAttackStateError attackError))
        {
            SetAttack(null);
        }
        else if (attackError == global::Weapon.CanAttackStateError.OverMaxCount)
        {
            // reloading
        }
        else if (attackError == global::Weapon.CanAttackStateError.Range)
        {
            // Range
        }
    }

    void SetAttack(SkillLoader.SkillToken token)
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
        var AttackAP = aph.GetAnimationPoint<PersonAnimationPoint>(0);
        SetAPs(AttackAP, prepareData.target, PersonAniState.StateKind.Attack, 0, false, true);
        AttackAP.EventTrigger = AttackTrigger; // for actual attack timing
        var skillToken = token == null ? skillLoader.UseSkill() : token;
        AttackAP.SkillData = skillToken.SkillData;
        skillToken.curLoopCount++;
        AttackAP.whenAnimationExitTime = () => { WhenExitTime(AttackAP, skillToken); }; // every exit time, the attack state will issue an AP
        SetAPH(aph, true);
    }

    public void AttackTrigger(int num)
    {
        Debug.Log("isIn");
    }

    public void WhenExitTime(AnimationPoint ap, SkillLoader.SkillToken token)
    {
        // checking loop
        if (ap.Weapon == Weapon)
        {
            if (token.maxLoopCount != 0)
            {
                if (token.curLoopCount <= token.maxLoopCount)
                {
                    SetAttack(token);
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
