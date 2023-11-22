using JExtentioner;
using System;
using System.Collections;
using UnityEngine;

public abstract class AniController : MonoBehaviour, IJobStarter<ModelAnimationPlayerJobManager.ModelHandlerJob>
{
    protected RagDollHandler ragDollHandler { set; get; }
    protected NaviController naviController;
    protected Animator animator;
    protected Transform headFollowTarget { set; get; } = null;
    float lookAtWeight = 0f;
    protected float animationPlayLimit = 0.85f;

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
            StartCoroutine(DoMakeCorrectTransform(ap));
        }
        else
        {
            reservedAP = ap;
            print("reservatiedAP");
        }
    }
    protected virtual IEnumerator DoMakeCorrectTransform(AnimationPoint ap)
    {
        var isPositionDone = false;
        var isRotationDone = false;
        var positionCorrect = StartCoroutine(DoPositionCorrectly(ap.CorrectedPosition, () => { isPositionDone = true; }));
        var rotationCorrect = StartCoroutine(DoRotationCorrectly(ap.transform.forward, () => { isRotationDone = true; }));

        yield return new WaitUntil(() => isPositionDone && isRotationDone);
        yield return new WaitUntil(() => IsWalkState() || IsAPReserved);

        if (!IsAPReserved)
            StartAni(ap);
        yield return null;
    }

    protected virtual bool IsWalkState() { return true; }

    protected virtual IEnumerator DoPositionCorrectly(Vector3 worldPosition, Action done)
    {
        var t = 0f;
        var maxT = 0.05f;
        var beforePosition = transform.position;
        while (t < maxT)
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
        var rotateDir = transform.forward.GetRotationDir(dir);
        var shouldBodyTurnWithAnimation = rotateDir >= bodyThreshold;

        if (shouldBodyTurnWithAnimation)
        {
            var during = GetMakeTurnDuring(rotateDir, out AnimationPoint playingAP);
            var totalAngle = Vector3.Angle(transform.forward, dir);
            var eachFrameAngle = totalAngle / (during / Time.fixedDeltaTime);
            for (float t = 0; t < during; t += Time.fixedDeltaTime)
            {
                transform.Rotate(isLeft ? Vector3.down : Vector3.up, eachFrameAngle);
                yield return new WaitForFixedUpdate();
            }
        }

        done?.Invoke();
        yield return null;
    }

    bool IsRatationDirLeft(Vector3 targetDir)
    {
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, targetDir);
        return dot < 0;
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

    protected virtual float GetMakeTurnDuring(float degree, out AnimationPoint playingAP) { playingAP = null; return 0; }
    protected abstract void StartAni(AnimationPoint actionPoint, bool shouldReturnAP = false);
    protected void StartAniTimeCount(AnimationPoint ap, bool shouldReturnAP, StateModule stateModule, AnimationEvent[] events)
    {
        PlayingAni = StartCoroutine(DoAnimationTimeCount(ap, shouldReturnAP, stateModule, events));
    }
    protected IEnumerator DoAnimationTimeCount(AnimationPoint ap, bool shouldReturnAP, StateModule stateModule, AnimationEvent[] events)
    {
        if (ap.IsUnLimited) yield break;

        ap.whenAnimationStart?.Invoke();

        var maxTime = Mathf.Lerp(0, ap.during, animationPlayLimit);
        int eventsCount = 0;
        for (float time = 0f; time < maxTime; time += Time.fixedDeltaTime)
        {
            if (events != null &&
                eventsCount < events.Length)
            {
                var targetEvent = events[eventsCount];
                if (targetEvent.time > time)
                {
                    ap.EventTrigger?.Invoke(targetEvent.stringParameter);
                    eventsCount++;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        ap.whenAnimationEnd?.Invoke();
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
            var ap = reservedAP;
            reservedAP = null;
            MakeCorrectTransform(ap);
        }
        else
        {
            if (shouldReadNextAction)
                modelHandlerJob.EndJob();
        }
        yield return null;
    }

    // this function only exist for hiding console log. basiclly not use. but dont remove.
    public void ExcuteDummyAniamtionEvent(string eventName) { }
}
