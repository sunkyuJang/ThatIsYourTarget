using System;
using UnityEngine;
using System.Collections;

public class Attack_PersonState : PersonState
{
    public enum APHDoneState { Attacking, Delaying, Non }
    public APHDoneState aphDoneState = APHDoneState.Non;
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
            aphDoneState = APHDoneState.Attacking;
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
        AttackAP.animationPointData.EventTrigger = AttackTrigger; // for actual attack timing
        var skillToken = token == null ? skillLoader.UseSkill() : token;
        AttackAP.animationPointData.SkillData = skillToken.SkillData;
        AttackAP.animationPointData.CanAnimationCancle = false;
        skillToken.curLoopCount++;

        // var usingAP = aph.GetAnimationPoint<PersonAnimationPoint>(1);
        // SetAPs(usingAP, prepareData.target, PersonAniState.StateKind.UsingWeapon, 0f, false, true);
        // usingAP.animationPointData.CanAnimationCancle = false;

        if (skillToken.SkillData.canLoop)
        {
            if (skillToken.maxLoopCount == 0)
            {
                skillToken.maxLoopCount = UnityEngine.Random.Range(0, 2);
            }
        }

        SetAPH(aph, true);
    }

    public void AttackTrigger(int num)
    {
        Debug.Log("isIn");
    }

    // public void WhenExitTime(AnimationPoint ap, SkillLoader.SkillToken token)
    // {
    //     // checking loop
    //     if (ap.animationPointData.Weapon == Weapon)
    //     {
    //         if (token.maxLoopCount != 0)
    //         {
    //             if (token.curLoopCount <= token.maxLoopCount)
    //             {
    //                 SetAttack(token);
    //             }
    //             else
    //             {
    //                 aphDoneState = APHDoneState.Delaying;
    //                 var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
    //                 var loopDelayAP = aph.GetAnimationPoint(0);
    //                 SetAPs(loopDelayAP, prepareData.target, PersonAniState.StateKind.UsingWeapon, 1f, false, true);
    //                 SetAPH(aph, true);
    //             }
    //             return;
    //         }
    //     }
    //     StartModule();
    // }

    protected override void AfterAPHDone()
    {
        switch (aphDoneState)
        {
            case APHDoneState.Attacking:
                if (IsInSight(prepareData.target))
                    StartModule();
                else
                    SetState(StateKinds.Tracking, prepareData);
                break;

            case APHDoneState.Delaying:
                StartModule();
                break;
        }

    }
    public override void Exit()
    {
        base.Exit();
    }
}
