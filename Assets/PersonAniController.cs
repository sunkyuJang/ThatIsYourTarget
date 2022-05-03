using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAniController : AniController
{
    Coroutine PlayingAni { set; get; }
    public bool IsPlayingAni { get { return PlayingAni != null; } }
    public enum AnimationsWithLevel { WalkAroundLevel = 0, SittingLevel, }
    public enum WalkLevel { Stop = 0, Walk, Run }
    public enum SittingLevel { Non = 0, High, Middle, Low }
    public enum AnimationsWithBool { ShouldStand, ShouldTurn, LookAround }

    public override void StartAni(ActionPoint actionPoint)
    {
        var ap = actionPoint as PersonActionPoint;
        switch (ap.state)
        {
            case (int)PersonActionPoint.StateKind.Sitting: SetSittingAnimation((SittingLevel)ap.sittingNum); break;
            case (int)PersonActionPoint.StateKind.LookAround: SetLookAroundAnimation(); break;
            case (int)PersonActionPoint.StateKind.Standing: StopMove(); break;//SetStandingAnimation(); break;
            case (int)PersonActionPoint.StateKind.PrepareAttack: SetPrepareAttack(ap.shouldReadyForBattle, ap.weaponLayer); break;
            default:
                //ap.StartTimeCount(MakeResetAni);
                break;
        }

        if (IsPlayingAni)
            StopCoroutine(PlayingAni);

        PlayingAni = StartCoroutine(DoAnimationTimeCount(ap.during));
    }

    IEnumerator DoAnimationTimeCount(float limitedTime)
    {
        var time = 0f;
        while (time < limitedTime)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        PlayingAni = null;

        MakeResetAni();
    }

    public void SetPrepareAttack(bool shouldPrepare, int weaponLayer)
    {
        if (shouldPrepare)
        {
            animator.SetLayerWeight(weaponLayer, 1);
        }
        else
        {
            MakeResetAni();
        }
    }

    public void SetSittingAnimation(SittingLevel sittingLevel)
    {
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)sittingLevel);
        StopMove();
    }

    public void SetLookAroundAnimation()
    {
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), true);
        StopMove();
    }

    // public void SetStandingAnimation()
    // {
    //     animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), true);
    //     StopMove();
    // }

    void StopMove()
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)WalkLevel.Stop);
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), true);
    }

    // void MakeMove(WalkLevel walkLevel)
    // {
    //     modelPhysicsController.naviController.TurnOnNavi(true);
    // }

    public void MakeResetAni()
    {
        StartCoroutine(DoResetAni());
    }

    IEnumerator DoResetAni()
    {
        var wasStanding = animator.GetInteger(AnimationsWithLevel.SittingLevel.ToString()) != 0;

        for (int i = 1; i < animator.layerCount; i++)
            animator.SetLayerWeight(i, 0);
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)SittingLevel.Non);
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), false);

        if (wasStanding)
            yield return new WaitForSeconds(1f);

        SetWalkState(WalkLevel.Walk);
        modelPhysicsController.ReadNextAction();
    }

    public void SetWalkState(WalkLevel walkLevel)
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)walkLevel);
    }
}
