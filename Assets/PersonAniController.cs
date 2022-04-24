using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAniController : AniController
{
    public enum AnimationsWithLevel { WalkAroundLevel = 0, SittingLevel, }
    public enum WalkLevel { Stop = 0, Walk, Run }
    public enum SittingLevel { Non = 0, High, Middle, Low }
    public enum AnimationsWithBool { ShouldStand, ShouldTurn, LookAround }
    public override void StartAni(ActionPoint actionPoint)
    {
        var ap = actionPoint as PersonActionPoint;
        switch (ap.state)
        {
            case (int)PersonActionPoint.StateKind.sitting: SetSittingAnimation((SittingLevel)ap.SittingNum); break;
            case (int)PersonActionPoint.StateKind.lookAround: SetLookAroundAnimation(); break;
            case (int)PersonActionPoint.StateKind.standing: SetStandingAnimation(); break;
            default:
                //ap.StartTimeCount(MakeResetAni);
                break;

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

    public void SetStandingAnimation()
    {
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), true);
        StopMove();
    }

    void StopMove()
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)WalkLevel.Stop);
        modelPhysicsController.naviController.TurnOnNavi(false);
    }

    void MakeMove(WalkLevel walkLevel)
    {
        modelPhysicsController.naviController.TurnOnNavi(true);
    }

    public void MakeResetAni()
    {
        StartCoroutine(DoResetAni());
    }

    IEnumerator DoResetAni()
    {
        var wasStanding = animator.GetInteger(AnimationsWithLevel.SittingLevel.ToString()) == 0;

        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)SittingLevel.Non);
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), false);

        if (wasStanding)
            yield return new WaitForSeconds(1f);

        modelPhysicsController.ReadNextAction();
    }

    public void SetWalkState(WalkLevel walkLevel)
    {
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)SittingLevel.Non);
    }

    new protected void FixedUpdate() { base.FixedUpdate(); }
}
