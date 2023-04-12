using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JMath;

public class AniController : MonoBehaviour, IJobStarter
{
    protected RagDollHandler ragDollHandler { set; get; }
    protected NaviController naviController;
    protected Animator animator;
    protected bool isPositionCorrect { set; get; } = false;
    protected bool isRotationCorrect { set; get; } = false;
    protected Transform headFollowTarget { set; get; } = null;
    float lookAtWeight = 0f;
    protected float animationPlayLimit = 0.85f;
    protected ActionPoint reservatiedAP { set; get; }
    Coroutine PlayingAni { set; get; }
    public bool IsPlayingAni { get { return PlayingAni != null; } }
    List<Coroutine> jobStopList = new List<Coroutine>();
    protected Coroutine ProcResetAni { set; get; }
    private ModelHandler.ModelHandlerJob modelHandlerJob { set; get; }
    bool ShouldReserveAP
    {
        get
        {
            return ProcResetAni != null;
        }
    }
    protected List<Coroutine> playingAniList { set; get; } = new List<Coroutine>();
    protected ActionPointHandler.WalkingState walkingState { set; get; }
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
    public bool IsSameSection(Job job)
    {
        return false;
    }
    public void StartJob(Job job)
    {
        if (job is ModelHandler.ModelHandlerJob)
        {
            modelHandlerJob = job as ModelHandler.ModelHandlerJob;
            var ap = modelHandlerJob.ap;

            StartAni(ap);
        }
    }
    void StartAni(ActionPoint ap)
    {
        if (!IsPlayingAni)
        {
            jobStopList.Add(StartCoroutine(DoMakeCorrect(ap)));
        }
        else
        {
            reservatiedAP = ap;
            print("reservatiedAP");
        }
    }
    protected virtual IEnumerator DoMakeCorrect(ActionPoint ap)
    {
        SetCorrectly(ap);
        yield return new WaitUntil(() => isPositionCorrect && isRotationCorrect);
        StartAni(ap);
        yield return null;
    }

    protected void SetCorrectly(ActionPoint ap)
    {
        jobStopList.Add(StartCoroutine(DoPositionCorrectly(ap.transform.position)));
        jobStopList.Add(StartCoroutine(DoRotationCorrectly(ap.transform.forward)));
    }

    protected virtual IEnumerator DoPositionCorrectly(Vector3 worldPosition)
    {
        var t = 0f;
        var maxT = 0.05f;
        var beforePosition = transform.position;
        isPositionCorrect = false;
        while (t < maxT)
        {
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
            var ratio = Mathf.InverseLerp(0, maxT, t);
            transform.position = Vector3.Lerp(beforePosition, Vector3Extentioner.GetOverrideY(worldPosition, beforePosition.y), ratio);
        }
        isPositionCorrect = true;
    }
    protected virtual IEnumerator DoRotationCorrectly(Vector3 dir)
    {
        isRotationCorrect = false;
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, dir);
        var isLeft = dot < 0;
        var rotateSpeed = 300f;
        var lastAngle = Vector3.Angle(transform.forward, dir);

        //Roughly
        while (true)
        {
            transform.Rotate(isLeft ? Vector3.down : Vector3.up, rotateSpeed * Time.fixedDeltaTime);
            var nowAngle = Vector3.Angle(transform.forward, dir);
            if (nowAngle > lastAngle) break;
            else lastAngle = nowAngle;
            yield return new WaitForFixedUpdate();
        }

        //Correctly
        if (Vector3.Angle(transform.forward, dir) * Mathf.Rad2Deg > 3f)
        {
            var t = 0f;
            var maxT = 1f;
            startForward = transform.forward;
            while (t < maxT)
            {
                var ratio = Mathf.InverseLerp(0, maxT, t);
                transform.forward = Vector3.Lerp(startForward, dir, ratio);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        isRotationCorrect = true;
        yield return null;
    }

    public void StopJob()
    {
        jobStopList.ForEach(x =>
        {
            if (x != null)
            {
                StopCoroutine(x);
            }
        });

        MakeResetAni(false);
    }

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

    public virtual void MakeTurn(float degree) { }
    public virtual void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false) { }
    protected void StartAniTimeCount(ActionPoint ap, bool shouldReturnAP, StateModule stateModule)
    {
        PlayingAni = StartCoroutine(DoAnimationTimeCount(ap, shouldReturnAP, stateModule));
    }
    protected IEnumerator DoAnimationTimeCount(ActionPoint ap, bool shouldReturnAP = false, StateModule stateModule = null)
    {
        if (ap.during < -1) yield return null;
        var maxTime = Mathf.Lerp(0, ap.during, animationPlayLimit);
        for (float time = 0f; time < maxTime && reservatiedAP == null; time += Time.fixedDeltaTime)
        {
            yield return new WaitForFixedUpdate();
        }

        MakeResetAni(!shouldReturnAP);

        if (shouldReturnAP)
            APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).ReturnTargetObj(ap.gameObject);
    }
    public void MakeResetAni(bool shouldReadNextAction = true, StateModule stateModule = null)
    {
        ProcResetAni = StartCoroutine(DoResetAni(shouldReadNextAction, stateModule));
    }
    protected virtual IEnumerator DoResetAni(bool shouldReadNextAction, StateModule stateModule)
    {
        PlayingAni = null;
        if (reservatiedAP == null)
        {
            if (shouldReadNextAction)
                modelHandlerJob.EndJob();
        }
        else
        {
            var ap = reservatiedAP;
            reservatiedAP = null;
            StartAni(ap);
        }
        yield return null;
    }
}
