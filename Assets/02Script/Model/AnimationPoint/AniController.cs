using JExtentioner;
using System;
using System.Collections;
using UnityEngine;
using RootMotion.FinalIK;
using System.Collections.Generic;
using SensorToolkit;

public abstract class AniController : MonoBehaviour, IJobStarter<ModelAnimationPlayerJobManager.ModelHandlerJob>
{
    protected RagDollHandler ragDollHandler { set; get; }
    protected NaviController naviController;
    protected Animator animator;
    protected Transform headFollowTarget { set; get; } = null;
    float lookAtWeight = 0f;
    protected float animationPlayLimit = 0.85f;

    // IK
    protected AimIK aimIK;
    protected LookAtIK lookAtIK;
    [SerializeField] private float IK_bodyThreshold = 0f;
    protected Coroutine DoTrackingBody { set; get; }

    // reserved Ani
    protected AnimationPoint reservedAP { set; get; }
    protected bool IsAPReserved { get { return reservedAP != null; } }

    // playing Ani
    Coroutine PlayingAni { set; get; }
    protected bool IsPlayingAni { get { return PlayingAni != null; } }

    private ModelAnimationPlayerJobManager.ModelHandlerJob modelHandlerJob { set; get; }
    protected AnimationPointHandler.WalkingState walkingState { set; get; }
    protected float bodyThreshold = 0f;
    protected StateModuleHandler stateModuleHandler { set; get; }
    protected virtual void Awake()
    {
        naviController = GetComponent<NaviController>();
        ragDollHandler = new RagDollHandler(transform);
        animator = GetComponent<Animator>();
        aimIK = GetComponent<AimIK>();
        lookAtIK = GetComponent<LookAtIK>();
        var FOVCollider = GetComponentInChildren<FOVCollider>();
        if (FOVCollider == null)
            Debug.Log("aniController cant find fovCollider");
        else
            IK_bodyThreshold = FOVCollider.FOVAngle * 0.5f;
    }

    protected virtual void Start()
    {
        StartCoroutine(DoWalking());
        StartCoroutine(DoHeadFollow());
        stateModuleHandler = GetStateModuleHandler();
    }

    protected abstract StateModuleHandler GetStateModuleHandler();

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

                var walkX = Mathf.Cos(degree * Mathf.Deg2Rad);
                var walkY = Mathf.Sin(degree * Mathf.Deg2Rad);
                animator.SetFloat("WalkY", walkX);
                animator.SetFloat("WalkX", walkY);
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
    public void StartJob(ModelAnimationPlayerJobManager.ModelHandlerJob job)
    {
        modelHandlerJob = job;
        walkingState = modelHandlerJob.walkingState;
        var ap = modelHandlerJob.ap;
        MakeCorrectTransform(ap);
    }
    void MakeCorrectTransform(AnimationPoint ap)
    {
        if (!IsPlayingAni)
        {
            Debug.Log("isin make Correct _ " + ap.animationPointData.state);
            PlayingAni = StartCoroutine(DoMakeCorrectTransform(ap));
        }
        else
        {
            Debug.Log("isin reserved _ " + ap.animationPointData.state);
            reservedAP = ap;
        }
    }
    protected virtual IEnumerator DoMakeCorrectTransform(AnimationPoint ap)
    {
        if (ap.animationPointData.LookAtTransform == null)
        {
            aimIK.solver.IKPositionWeight = 0;
            aimIK.enabled = false;

            var isPositionDone = false;
            var isRotationDone = false;
            var positionCorrect = StartCoroutine(DoPositionCorrectly(ap.animationPointData.CorrectedPosition, () => { isPositionDone = true; }));
            var rotationCorrect = StartCoroutine(DoRotationCorrectly(ap.transform.forward, () => { isRotationDone = true; }));

            yield return new WaitUntil(() => isPositionDone && isRotationDone);
            yield return new WaitUntil(() => IsWalkState());
        }
        else
        {
            if (DoTrackingBody != null)
                StopCoroutine(DoTrackingBody);

            DoTrackingBody = StartCoroutine(DoTracking(ap));
        }

        StartAni(ap);

        yield return null;
    }

    protected virtual bool IsWalkState() { return true; }

    protected virtual IEnumerator DoPositionCorrectly(Vector3 worldPosition, Action done)
    {
        var t = 0f;
        var maxT = 0.05f;
        var beforePosition = transform.position;
        while (t < maxT && !IsAPReserved)
        {
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
            var ratio = Mathf.InverseLerp(0, maxT, t);
            transform.position = Vector3.Lerp(beforePosition, worldPosition.GetOverrideY(beforePosition.y), ratio);
        }

        done?.Invoke();
    }
    protected virtual IEnumerator DoRotationCorrectly(Vector3 dir, Action done)
    {
        var isLeft = IsRatationDirLeft(dir);
        var rotateDir = MathF.Abs(transform.forward.GetRotationDir(dir));
        var shouldBodyTurnWithAnimation = rotateDir >= bodyThreshold;

        if (shouldBodyTurnWithAnimation)
        {
            var during = GetMakeTurnDuring(rotateDir * (isLeft ? -1 : 1));
            var totalAngle = Vector3.Angle(transform.forward, dir);
            var eachFrameAngle = totalAngle / (during / Time.fixedDeltaTime);
            for (float t = 0; t < during && !IsAPReserved; t += Time.fixedDeltaTime)
            {
                transform.Rotate(isLeft ? Vector3.down : Vector3.up, eachFrameAngle);
                yield return new WaitForFixedUpdate();
            }
            transform.Rotate(isLeft ? Vector3.down : Vector3.up, eachFrameAngle);
        }

        done?.Invoke();
        yield return null;
    }

    IEnumerator DoTracking(AnimationPoint ap)
    {
        var rotationSpeed = 2f;
        var animationEnded = false;
        aimIK.solver.target = ap.animationPointData.LookAtTransform;
        aimIK.solver.IKPositionWeight = 1;
        aimIK.enabled = true;
        ap.animationPointData.whenAnimationEnd += () => { animationEnded = true; };

        while (true && ap.animationPointData.LookAtTransform != null)
        {
            var limit = IK_bodyThreshold;
            var targetDir = aimIK.solver.target.position.ExceptVector3Property(1) - transform.position.ExceptVector3Property(1);
            var targetAngle = transform.forward.GetRotationDir(targetDir);
            if (Mathf.Abs(targetAngle) >= limit)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }

            if (animationEnded)
            {
                if (!IsAPReserved
                    || IsAPReserved && !(reservedAP.animationPointData.LookAtTransform == ap.animationPointData.LookAtTransform))
                    break;
            }
            yield return new WaitForFixedUpdate();
        }

        if (ap.animationPointData.LookAtTransform == null)
            aimIK.enabled = false;
    }

    bool IsRatationDirLeft(Vector3 targetDir)
    {
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, targetDir);
        return dot < 0;
    }

    public void StopJob() { }
    protected virtual float GetMakeTurnDuring(float degree) { return 0; }
    protected abstract void StartAni(AnimationPoint actionPoint, bool shouldReturnAP = false);
    protected void StartAniTimeCount(AnimationPoint ap, bool shouldReturnAP, StateModule stateModule, List<float> events)
    {
        StartCoroutine(DoAnimationTimeCount(ap, shouldReturnAP, stateModule, events));
    }
    protected IEnumerator DoAnimationTimeCount(AnimationPoint ap, bool shouldReturnAP, StateModule stateModule, List<float> events)
    {
        if (ap.animationPointData.IsUnLimited) yield break;

        ap.animationPointData.whenAnimationStart?.Invoke();

        List<KeyValuePair<float, string>> exitEvent = null;
        if (ap.animationPointData.SkillData != null)
            exitEvent = ap.GetExitAniEvent(ap.animationPointData.SkillData.keyName);

        var triggeredExitEvent = false;

        int eventsCount = 0;
        for (float time = 0f; time < ap.animationPointData.during && !(IsAPReserved && ap.animationPointData.CanAnimationCancle); time += Time.fixedDeltaTime)
        {
            if (events != null &&
                eventsCount < events.Count)
            {
                var targetEvent = events[eventsCount];
                if (targetEvent < time)
                {
                    ap.animationPointData.EventTrigger?.Invoke(eventsCount);
                    eventsCount++;
                }
            }

            if (exitEvent != null && exitEvent.Count > 0 && !triggeredExitEvent)
            {
                var exitTime = exitEvent[0].Key;
                var rateExitTime = Mathf.Lerp(0, exitTime, 0.5f);
                if (rateExitTime < time)
                {
                    triggeredExitEvent = true;
                    ap.animationPointData.whenAnimationExitTime?.Invoke();
                }
            }

            yield return new WaitForFixedUpdate();
        }

        Debug.Log("isin timecountinf done + " + ap.animationPointData.state + "//" + ap.animationPointData.during + "//" + IsAPReserved + "//" + ap.animationPointData.CanAnimationCancle);

        ap.animationPointData.whenAnimationEnd?.Invoke();
        MakeResetAni(!shouldReturnAP, stateModule);

        if (shouldReturnAP)
            APHManager.Instance.ReturnAP(ap.gameObject);
    }
    protected void MakeResetAni(bool shouldReadNextAction, StateModule stateModule)
    {
        StartCoroutine(DoResetAni(shouldReadNextAction, stateModule));
    }
    protected virtual IEnumerator DoResetAni(bool shouldReadNextAction, StateModule stateModule)
    {
        yield return new WaitUntil(() => IsWalkState());

        PlayingAni = null;
        if (IsAPReserved)
        {
            RunReservedAP();
        }
        else
        {
            if (shouldReadNextAction)
                modelHandlerJob.EndJob();
        }
        yield return null;
    }

    protected void RunReservedAP()
    {
        var ap = reservedAP;
        reservedAP = null;
        PlayingAni = null;

        Debug.Log("isin reserved Start + : " + ap.animationPointData.state);
        MakeCorrectTransform(ap);
    }

    // this function only exist for hiding console log. basiclly not use. but dont remove.
    public void ExcuteDummyAniamtionEvent(string eventName) { }
}
