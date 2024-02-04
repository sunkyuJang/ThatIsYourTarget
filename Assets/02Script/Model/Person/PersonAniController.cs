using System.Collections;
using UnityEngine;

public class PersonAniController : AniController
{
    public int WeaponMotionLayer = 1;

    public GameObject personNeck;
    PersonAniStateModuleHandler moduleHandler => base.stateModuleHandler as PersonAniStateModuleHandler;

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
        return new PersonAniStateModuleHandler(animator, ragDollHandler);
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
        if (actionPoint is PersonAnimationPoint)
        {
            var ap = actionPoint as PersonAnimationPoint;
            var module = moduleHandler.GetModule(ap.State);
            if (module != null && module is PersonAniState aniModule)
            {
                aniModule.SetAP(ap);
                aniModule.TryEnter<StateModule.PrepareData>();

                // walk animation should stop for other animation
                SetWalkModule(AnimationPointHandler.WalkingState.Non);

                var events = ap.GetAnimationEvent();
                StartAniTimeCount(ap, shouldReturnAP, module, events);
            }
        }
    }

    void SetWalkModule(AnimationPointHandler.WalkingState walkingState)
    {
        var walkModule = moduleHandler.GetModule(PersonAniState.StateKind.Walk);
        var usingModule = moduleHandler.GetModule(PersonAniState.StateKind.UsingWeapon);

        if (CanModuleRun(walkModule) && walkModule is Walk_PersonAniState)
        {
            var walkingModule = walkModule as Walk_PersonAniState;
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
            if (stateModule is PersonAniState)
            {
                (stateModule as PersonAniState).Exit();
            }
        }

        SetWalkModule(walkingState);
        yield return StartCoroutine(base.DoWaitUntilAnimationReset(null));
    }

    protected override AnimationPoint GetTurnAroundAP(float degree)
    {
        var ap = APHManager.Instance.GetNewAP<PersonAnimationPoint>();
        ap.animationPointData.state = (int)PersonAniState.StateKind.TurnAround;
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

    bool CanModuleRun(StateModule module)
    {
        return module != null && module is PersonAniState;
    }

}
