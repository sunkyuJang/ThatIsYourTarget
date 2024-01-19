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

        Debug.Log("isin attacking in");
        aphDoneState = APHDoneState.Attacking;
        if (Weapon.CanAttack(prepareData.target, out Weapon.CanAttackStateError attackError))
        {
            SetAttack();
        }
        else if (attackError == global::Weapon.CanAttackStateError.OverMaxCount)
        {
            // reloading
        }
    }

    void SetAttack()
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);

        var AttackAP = aph.GetAnimationPoint<PersonAnimationPoint>(0);
        SetAPs(AttackAP, prepareData.target, PersonAniState.StateKind.Attack, 0, false, true);
        AttackAP.animationPointData.EventTrigger = AttackTrigger; // for actual attack timing
        var skillToken = skillLoader.UseSkill();
        AttackAP.animationPointData.SkillData = skillToken.SkillData;
        AttackAP.animationPointData.CanAnimationCancle = false;
        skillToken.curLoopCount++;

        AttackAP.animationPointData.EventTrigger += (int i) => { AttackTrigger(i); };
        AttackAP.animationPointData.LookAtTransform = prepareData.target;
        AttackAP.animationPointData.StoppingDistance = Weapon.range;

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

    protected override void AfterAPHDone()
    {
        switch (aphDoneState)
        {
            case APHDoneState.Attacking:
                if (IsInSight(prepareData.target))
                {
                    Debug.Log("isin aph Done to attack");
                    StartModule();
                }
                else
                {
                    Debug.Log("isin aph Done to tracking");
                    SetState(StateKinds.Tracking, new PersonPrepareData(prepareData.target));
                }

                break;

            case APHDoneState.Delaying:
                StartModule();
                break;
        }

    }
    public override void Exit()
    {
        prepareData = null;
        base.Exit();
    }
}
