using System.Collections;
using UnityEngine;

public class PersonAniController : AniController
{
    public GameObject personNeck;
    PersonAniStateModuleHandler moduleHandler => base.stateModuleHandler as PersonAniStateModuleHandler;
    protected override void Start()
    {
        base.Start();
        bodyThreshold = 5f;
    }
    protected override StateModuleHandler GetStateModuleHandler()
    {
        return new PersonAniStateModuleHandler(animator);
    }
    protected override bool IsWalkState()
    {
        return
            animator.GetCurrentAnimatorStateInfo(0).IsName("WalkAround") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("RunningAround");
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
        var module = moduleHandler.GetModule(PersonAniState.StateKind.Walk);

        if (CanModuleRun(module) && module is Walk_PersonAniState)
        {
            var walkingModule = module as Walk_PersonAniState;
            walkingModule.SetWalkState(walkingState);
            module.TryEnter<StateModule.PrepareData>(null);
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
    protected override IEnumerator DoResetAni(bool shouldReadNextAction, StateModule stateModule = null)
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
        StartCoroutine(base.DoResetAni(shouldReadNextAction, null));

        yield return null;
    }

    protected override float GetMakeTurnDuring(float degree, out AnimationPoint ap)
    {
        ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonAnimationPoint>();
        (ap as PersonAnimationPoint).State = PersonAniState.StateKind.TurnAround;
        ap.targetDegree = degree;
        ap.during = ap.GetAnimationClipLength(GetStateNameByDegree(ap.targetDegree));
        StartAni(ap, true);
        return ap.during;
    }

    string GetStateNameByDegree(float degree)
    {
        if (degree >= 0)
        {
            return degree > 135f ? "LongTurnR" : "TurnR";
        }
        else
        {
            return degree < -135f ? "TurnL" : "LongTurnL";
        }
    }

    bool CanModuleRun(StateModule module)
    {
        return module != null && module is PersonAniState;
    }

    // public ActionPoint MakeHeadTurn()
    // {
    //     var ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonActionPoint>();
    //     ap.State = PersonAniController.StateKind.TurnHead;
    //     ap.during = 3f;
    //     StartAni(ap, true);

    //     return ap;
    // }
}
