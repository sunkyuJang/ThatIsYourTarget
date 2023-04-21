using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMath;

public class AniController : MonoBehaviour, IJobStarter
{
    protected RagDollHandler ragDollHandler { set; get; }
    protected NaviController naviController;
    protected Animator animator;
    protected Transform headFollowTarget { set; get; } = null;
    float lookAtWeight = 0f;
    protected float animationPlayLimit = 0.85f;
    protected ActionPoint reservedAP { set; get; }
    protected bool IsAPReserved { get { return reservedAP != null; } }
    Coroutine PlayingAni { set; get; }
    protected bool IsPlayingAni { get { return PlayingAni != null; } }
    private ModelHandler.ModelHandlerJob modelHandlerJob { set; get; }
    protected ActionPointHandler.WalkingState walkingState { set; get; }
    protected float bodyThreshold = 0f;
    protected virtual void Awake()
    {
        ragDollHandler = GetComponent<RagDollHandler>();
        var naviController = GetComponent<NaviController>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        StartCoroutine(DoWalking());
        StartCoroutine(DoHeadFollow());
    }

    IEnumerator DoWalking()
    {
        while (true)
        {
            var direction = naviController.GetNaviDirection();
            if (direction == Vector3.zero)
            {
                animator.SetFloat("WalkY", 0f);
                animator.SetFloat("WalkX", 0f);
            }
            else
            {
                var degree = Mathf.Round(Vector3.Angle(transform.forward, direction));
                var cross = Vector3.Cross(transform.forward, direction);
                degree *= cross.y >= 0 ? 1 : -1;

                animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
                animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DoHeadFollow()
    {
        lookAtWeight = 0f;
        while (animator.isHuman)
        {
            yield return new WaitForFixedUpdate();
            if (headFollowTarget == null)
            {
                lookAtWeight = Mathf.Lerp(lookAtWeight, 0, Time.deltaTime * 2.5f);
            }
            else
            {
                lookAtWeight = Mathf.Lerp(lookAtWeight, 1, Time.deltaTime * 2.5f);
            }
        }
    }
    public void StartJob(Job job)
    {
        if (job is ModelHandler.ModelHandlerJob)
        {
            modelHandlerJob = job as ModelHandler.ModelHandlerJob;
            var ap = modelHandlerJob.ap;

            MakeCorrectTransform(ap);
        }
    }
    void MakeCorrectTransform(ActionPoint ap)
    {
        if (!IsPlayingAni)
        {
            StartCoroutine(DoMakeCorrectTransform(ap));
        }
        else
        {
            reservedAP = ap;
            print("reservatiedAP");
        }
    }
    protected virtual IEnumerator DoMakeCorrectTransform(ActionPoint ap)
    {
        var positionCorrect = StartCoroutine(DoPositionCorrectly(ap.transform.position));
        var rotationCorrect = StartCoroutine(DoRotationCorrectly(ap.transform.forward));

        yield return positionCorrect;
        yield return rotationCorrect;
        yield return new WaitUntil(() => IsWalkState());

        if (!IsAPReserved)
            StartAni(ap);
        yield return null;
    }

    protected virtual bool IsWalkState() { return true; }

    protected virtual IEnumerator DoPositionCorrectly(Vector3 worldPosition)
    {
        var t = 0f;
        var maxT = 0.05f;
        var beforePosition = transform.position;
        while (t < maxT || !IsAPReserved)
        {
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
            var ratio = Mathf.InverseLerp(0, maxT, t);
            transform.position = Vector3.Lerp(beforePosition, Vector3Extentioner.GetOverrideY(worldPosition, beforePosition.y), ratio);
        }
    }
    protected virtual IEnumerator DoRotationCorrectly(Vector3 dir)
    {
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, dir);
        var isLeft = dot < 0;
        var rotateDir = Vector3Extentioner.GetRotationDir(transform.forward, dir);

        var shouldBodyTurnWithAnimation = rotateDir >= bodyThreshold;

        if (shouldBodyTurnWithAnimation)
        {
            var during = GetMakeTurnDuring(rotateDir);

            var rotateTime = Mathf.Lerp(0, during, 0.45f);
            var totalAngle = Vector3.Angle(transform.forward, dir);
            var eachFrameAngle = totalAngle / (rotateTime / Time.fixedDeltaTime);
            for (float t = 0; t < during || !IsAPReserved; t += Time.fixedDeltaTime)
            {
                if (t < rotateTime)
                    transform.Rotate(isLeft ? Vector3.down : Vector3.up, eachFrameAngle);
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;
    }

    public void StopJob() { }
    private void OnAnimatorIK(int layer)
    {
        if (animator)
        {
            if (headFollowTarget != null)
            {
                animator.SetLookAtWeight(lookAtWeight);
                animator.SetLookAtPosition(headFollowTarget.transform.position);
            }
        }
    }

    protected virtual float GetMakeTurnDuring(float degree) { return 0; }
    protected virtual void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false) { }
    protected void StartAniTimeCount(ActionPoint ap, bool shouldReturnAP, StateModule stateModule)
    {
        PlayingAni = StartCoroutine(DoAnimationTimeCount(ap, shouldReturnAP, stateModule));
    }
    protected IEnumerator DoAnimationTimeCount(ActionPoint ap, bool shouldReturnAP = false, StateModule stateModule = null)
    {
        if (ap.during < -1) yield return null;
        var maxTime = Mathf.Lerp(0, ap.during, animationPlayLimit);
        for (float time = 0f; time < maxTime && !IsAPReserved; time += Time.fixedDeltaTime)
        {
            yield return new WaitForFixedUpdate();
        }

        MakeResetAni(!shouldReturnAP);

        if (shouldReturnAP)
            APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).ReturnTargetObj(ap.gameObject);
    }
    protected void MakeResetAni(bool shouldReadNextAction = true, StateModule stateModule = null)
    {
        StartCoroutine(DoResetAni(shouldReadNextAction, stateModule));
    }
    protected virtual IEnumerator DoResetAni(bool shouldReadNextAction, StateModule stateModule)
    {
        yield return new WaitUntil(() => IsWalkState());

        PlayingAni = null;
        if (IsAPReserved)
        {
            if (shouldReadNextAction)
                modelHandlerJob.EndJob();
        }
        else
        {
            var ap = reservedAP;
            reservedAP = null;
            MakeCorrectTransform(ap);
        }
        yield return null;
    }
}
