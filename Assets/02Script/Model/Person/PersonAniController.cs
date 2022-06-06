using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAniController : AniController
{
    public GameObject personNeck;
    Coroutine PlayingAni { set; get; }
    new public bool IsPlayingAni { get { return PlayingAni != null; } }
    public enum AnimationsWithLevel { WalkAroundLevel = 0, SittingLevel, }
    public enum AnimationsWithFloat { TurnDegree }
    public enum WalkLevel { Stop = 0, Walk, Run }
    public enum SittingLevel { Non = 0, High, Middle, Low }
    public enum AnimationsWithBool { ShouldStand, LookAround, ShouldSurprize }

    public override void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false)
    {
        var ap = actionPoint as PersonActionPoint;
        switch (ap.State)
        {
            case PersonActionPoint.StateKind.Sitting: SetSittingAnimation((SittingLevel)ap.sittingNum); break;
            case PersonActionPoint.StateKind.LookAround: SetLookAroundAnimation(); break;
            case PersonActionPoint.StateKind.Standing: StopMove(); break;
            case PersonActionPoint.StateKind.PrepareAttack: SetPrepareAttack(ap.shouldReadyForBattle, ap.weaponLayer); break;
            case PersonActionPoint.StateKind.Surprize: SetSurprizeAnimation(); break;
            case PersonActionPoint.StateKind.TurnAround: SetTurnAroundAnimation(ap); break;
            case PersonActionPoint.StateKind.TurnHead: SetTurnHead(ap); break;
            default:
                break;
        }

        if (IsPlayingAni)
            StopCoroutine(PlayingAni);

        PlayingAni = StartCoroutine(DoAnimationTimeCount(ap, shouldReturnAP));
    }

    IEnumerator DoAnimationTimeCount(ActionPoint actionPoint, bool shouldReturnAP = false)
    {
        if (actionPoint.during < -1) yield return null;
        var time = 0f;
        while (time < actionPoint.during)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        PlayingAni = null;

        if (shouldReturnAP)
            APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).ReturnTargetObj(actionPoint.gameObject);

        MakeResetAni(!shouldReturnAP);
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

    public void SetSurprizeAnimation()
    {
        animator.SetBool(AnimationsWithBool.ShouldSurprize.ToString(), true);
        StopMove(false);
    }

    void StopMove(bool shoulPlayStand = true)
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)WalkLevel.Stop);
        if (shoulPlayStand)
            animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), true);
    }

    public void SetTurnAroundAnimation(ActionPoint ap)
    {
        animator.SetFloat(AnimationsWithFloat.TurnDegree.ToString(), ap.targetDegree);
        StopMove(false);
    }
    public void SetTurnHead(ActionPoint ap)
    {
        headFollowTarget = ap.transform;
        StopMove(false);
    }

    public void MakeResetAni(bool shouldReadNextAction = true)
    {
        StartCoroutine(DoResetAni(shouldReadNextAction));
    }

    IEnumerator DoResetAni(bool shouldReadNextAction)
    {
        var wasStanding = animator.GetInteger(AnimationsWithLevel.SittingLevel.ToString()) != 0;

        for (int i = 1; i < animator.layerCount; i++)
            animator.SetLayerWeight(i, 0);
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)SittingLevel.Non);
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldSurprize.ToString(), false);
        animator.SetFloat(AnimationsWithFloat.TurnDegree.ToString(), 361f);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
        SetWalkState(WalkLevel.Walk);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("WalkAround") ||
                                            animator.GetCurrentAnimatorStateInfo(0).IsName("RunningAround"));

        if (shouldReadNextAction)
            modelPhysicsController.ReadNextAction();
    }

    public void SetWalkState(WalkLevel walkLevel)
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)walkLevel);
    }

    protected override IEnumerator DoMakeCorrect(ActionPoint ap)
    {
        SetCorrectly(ap);

        yield return new WaitUntil(() => isPositionCorrect && isRotationCorrect);

        StartAni(ap);
    }

    protected override IEnumerator DoRotationCorrectly(Vector3 dir)
    {
        isRotationCorrect = false;
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, dir);
        var isLeft = dot < 0;
        var rotateSpeed = 300f;
        var lastAngle = Vector3.Angle(transform.forward, dir);

        // make correction for animation float
        {
            var lastAngleABS = Mathf.Abs(lastAngle);
            if (lastAngleABS == 0f || lastAngleABS == 100 || lastAngleABS == 360)
                lastAngle += lastAngle >= 0 ? 1 : -1;
        }

        var limitDegreeOfHead = 80f;
        var shouldBodyTurnWithAnimation = lastAngle >= limitDegreeOfHead;

        if (shouldBodyTurnWithAnimation)
        {
            var ap = MakeTurn(lastAngle);

            //Roughly
            // while (true)
            // {
            //     transform.Rotate(isLeft ? Vector3.down : Vector3.up, rotateSpeed * Time.fixedDeltaTime);
            //     var nowAngle = Vector3.Angle(transform.forward, dir);
            //     if (nowAngle > lastAngle) break;
            //     else lastAngle = nowAngle;
            //     yield return new WaitForFixedUpdate();
            // }

            //Correctly
            // if (Vector3.Angle(transform.forward, dir) * Mathf.Rad2Deg > 3f)
            // {
            //     var t = 0f;
            //     var maxT = 1f;
            //     startForward = transform.forward;
            //     while (t < maxT)
            //     {
            //         var ratio = Mathf.InverseLerp(0, maxT, t);
            //         transform.forward = Vector3.Lerp(startForward, dir, ratio);
            //         t += Time.fixedDeltaTime;
            //         yield return new WaitForFixedUpdate();
            //     }
            // }

            var totalAngle = Vector3.Angle(transform.forward, dir);
            var eachFrameAngle = totalAngle / (ap.during / Time.fixedDeltaTime);
            for (float t = 0; t < ap.during; t += Time.fixedDeltaTime)
            {
                transform.Rotate(isLeft ? Vector3.down : Vector3.up, eachFrameAngle);
                yield return new WaitForFixedUpdate();
            }

            //yield return new WaitUntil(() => IsPlayingAni(0, GetStateNameByDegree(lastAngle)));
        }

        isRotationCorrect = true;
        yield return null;
    }
    new public ActionPoint MakeTurn(float degree)
    {
        var ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonActionPoint>();
        ap.State = PersonActionPoint.StateKind.TurnAround;
        ap.targetDegree = degree;
        ap.during = GetLength(GetStateNameByDegree(ap.targetDegree));
        StartAni(ap, true);
        return ap;
    }

    string GetStateNameByDegree(float degree)
    {
        if (degree >= 0)
        {
            return degree > 100f ? "LongTurnR" : "TurnR";
        }
        else
        {
            return degree < -100 ? "TurnL" : "LongTurnL";
        }
    }

    public ActionPoint MakeHeadTurn()
    {
        var ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonActionPoint>();
        ap.State = PersonActionPoint.StateKind.TurnHead;
        ap.during = 3f;
        StartAni(ap, true);

        return ap;
    }
}
