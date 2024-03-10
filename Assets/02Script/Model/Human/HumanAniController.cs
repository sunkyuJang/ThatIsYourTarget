using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JExtentioner;
using UnityEngine;

public class HumanAniController : AniController
{
    public int WeaponMotionLayer = 1;

    public GameObject personNeck;
    HumanAniStateModuleHandler moduleHandler => base.stateModuleHandler as HumanAniStateModuleHandler;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        bodyThreshold = 5f;
    }
    protected override StateModuleHandler GetStateModuleHandler()
    {
        return new HumanAniStateModuleHandler(animator, ragDollHandler);
    }
    protected override bool IsWalkState()
    {
        return
            (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkAround") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("RunningAround"))
            && !animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack");
    }

    protected override void StartAni(AnimationPoint actionPoint, bool shouldReturnAP = false)
    {
        if (actionPoint is HumanAnimationPoint)
        {
            var ap = actionPoint as HumanAnimationPoint;
            var module = moduleHandler.GetModule(ap.State);
            if (module != null && module is HumanAniState aniModule)
            {
                aniModule.SetAP(ap);
                aniModule.TryEnter<StateModule.PrepareData>();

                // walk animation should stop for other animation
                SetWalkModule(AnimationPointHandler.WalkingState.Non);

                List<float> events = null;
                List<KeyValuePair<float, string>> exitEvent = null;
                if (ap.animationPointData.SkillData == null)
                {
                    events = ap.GetAnimationEvent();
                }
                if (ap.animationPointData.SkillData != null)
                {
                    events = ap.GetAnimationEvent(ap.animationPointData.SkillData.keyName);
                    exitEvent = ap.GetExitAniEvent(ap.animationPointData.SkillData.keyName);
                }
                StartAniTimeCount(ap, shouldReturnAP, module, events, exitEvent);
            }
        }
    }

    void SetWalkModule(AnimationPointHandler.WalkingState walkingState)
    {
        var walkModule = moduleHandler.GetModule(HumanAniState.StateKind.Walk);
        var usingModule = moduleHandler.GetModule(HumanAniState.StateKind.UsingWeapon);

        if (CanModuleRun(walkModule) && walkModule is Walk_HumanAniState)
        {
            var walkingModule = walkModule as Walk_HumanAniState;
            walkingModule.SetWalkState(walkingState);
            walkModule.TryEnter<StateModule.PrepareData>(null);

            if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
            {
                usingModule.TryEnter<StateModule.PrepareData>();
            }
        }
        else
        {
            Debug.Log("there has no walk module");
        }
    }

    public void SetTurnHead(AnimationPoint ap)
    {
        headFollowTarget = ap.transform;
    }
    protected override IEnumerator DoWaitUntilAnimationReset(StateModule stateModule)
    {
        if (stateModule == null)
        {
            // activate reset module.
        }
        else
        {
            if (stateModule is HumanAniState)
            {
                (stateModule as HumanAniState).Exit();
            }
        }

        SetWalkModule(walkingState);
        yield return StartCoroutine(base.DoWaitUntilAnimationReset(null));
    }

    protected override AnimationPoint GetTurnAroundAP(float degree)
    {
        var ap = APHManager.Instance.GetNewAP<HumanAnimationPoint>();
        ap.animationPointData.state = (int)HumanAniState.StateKind.TurnAround;
        ap.animationPointData.targetDegree = degree;
        ap.animationPointData.during = ap.GetAnimationClipLength(GetStateNameByDegree(ap.animationPointData.targetDegree));
        return ap;
    }

    string GetStateNameByDegree(float degree)
    {
        if (degree >= 0)
        {
            return degree > 135f ? "LongTurnR" : "TurnR";
        }
        else
        {
            return degree > -135f ? "TurnL" : "LongTurnL";
        }
    }

    protected override IEnumerator DoRotationCorrectly(Vector3 dir, float during)
    {
        var hip = animator.GetBoneTransform(HumanBodyBones.Hips);
        var hipForward = hip.up;
        transform.forward = hipForward.ExceptVector3Property(1);
        hip.up = hipForward;

        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(base.DoRotationCorrectly(dir, during));
    }

    bool CanModuleRun(StateModule module)
    {
        return module != null && module is HumanAniState;
    }

}
