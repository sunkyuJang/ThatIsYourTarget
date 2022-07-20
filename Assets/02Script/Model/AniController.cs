using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JMath;

public class AniController : MonoBehaviour
{
    protected RagDollHandler ragDollHandler { set; get; }
    protected ModelPhysicsController modelPhysicsController;
    protected Animator animator;
    protected bool isPositionCorrect { set; get; } = false;
    protected bool isRotationCorrect { set; get; } = false;
    protected Transform headFollowTarget { set; get; } = null;
    float lookAtWeight = 0f;
    protected float animationPlayLimit = 0.85f;
    protected ActionPoint reservatiedAP { set; get; }
    Coroutine PlayingAni { set; get; }
    public bool IsPlayingAni { get { return PlayingAni != null; } }
    protected Coroutine ProcResetAni { set; get; }
    bool ShouldReserveAP
    {
        get
        {
            return ProcResetAni != null;
        }
    }

    // public bool IsPlayingAni(int layer, string stateName)
    // {
    //     var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
    //     return stateInfo.IsName(stateName);
    // }
    protected void Awake()
    {
        ragDollHandler = GetComponent<RagDollHandler>();
        modelPhysicsController = GetComponent<ModelPhysicsController>();
        animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        StartCoroutine(DoWalking());
        StartCoroutine(DoHeadFollow());
    }

    IEnumerator DoWalking()
    {
        while (true)
        {
            if (modelPhysicsController.naviController.navMeshAgent != null)
            {
                if (modelPhysicsController.naviController.navMeshAgent.isOnNavMesh
                    && !modelPhysicsController.naviController.navMeshAgent.isStopped)
                {
                    var NavMeshAgent = modelPhysicsController.naviController.navMeshAgent;

                    var degree = Mathf.Round(Vector3.Angle(transform.forward, NavMeshAgent.velocity.normalized));
                    var cross = Vector3.Cross(transform.forward, NavMeshAgent.velocity.normalized);
                    degree *= cross.y >= 0 ? 1 : -1;

                    animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
                    animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));
                }
                else
                {
                    animator.SetFloat("WalkY", 0f);
                    animator.SetFloat("WalkX", 0f);
                }
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
    public void MakeCorrect(ActionPoint ap)
    {
        if (PlayingAni == null)
        {
            StartCoroutine(DoMakeCorrect(ap));
        }
        else
        {
            reservatiedAP = ap;
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
        SetPositionCorrectly(ap.transform.position);
        SetRotationCorrectly(ap.transform.forward);
    }

    protected void SetPositionCorrectly(Vector3 worldPosition)
    {
        StartCoroutine(DoPositionCorrectly(worldPosition));
    }

    protected virtual IEnumerator DoPositionCorrectly(Vector3 worldPosition)
    {
        var t = 0f;
        var maxT = 0.5f;
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

    protected void SetRotationCorrectly(Vector3 dir)
    {
        StartCoroutine(DoRotationCorrectly(dir));
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
    protected void StartAniTimeCount(float during, bool shouldReturnAP)
    {
        if (IsPlayingAni)
            StopCoroutine(PlayingAni);

        PlayingAni = StartCoroutine(DoAnimationTimeCount(during, shouldReturnAP));
    }
    protected IEnumerator DoAnimationTimeCount(float during, bool shouldReturnAP = false)
    {
        if (during < -1) yield return null;
        var maxTime = Mathf.Lerp(0, during, animationPlayLimit);
        for (float time = 0f; time < maxTime && reservatiedAP == null; time += Time.fixedDeltaTime)
        {
            yield return new WaitForFixedUpdate();
        }
        PlayingAni = null;

        MakeResetAni(!shouldReturnAP);
    }
    public void MakeResetAni(bool shouldReadNextAction = true)
    {
        ProcResetAni = StartCoroutine(DoResetAni(shouldReadNextAction));
    }
    protected virtual IEnumerator DoResetAni(bool shouldReadNextAction)
    {
        if (reservatiedAP == null)
        {
            if (shouldReadNextAction)
                modelPhysicsController.ReadNextAction();
        }
        else
        {
            var ap = reservatiedAP;
            reservatiedAP = null;
            MakeCorrect(ap);
        }
        yield return null;
    }
}
