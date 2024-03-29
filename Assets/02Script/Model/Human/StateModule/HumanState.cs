using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class HumanState : StateModule
{
    public enum StateKinds
    {
        Normal,
        Sensed,
        Curiousity,
        //HoldingWeapon,
        Tracking,
        Patrol,
        Attack,
        Hit,
        Dead,
        Non
    }
    readonly public static List<HumanAniState.StateKind> SensedLockStateList = new List<HumanAniState.StateKind>()
    {
        HumanAniState.StateKind.Surprize,
    };
    public static List<StateKinds> CanYeildList = new List<StateKinds>() { StateKinds.Normal, StateKinds.Sensed, StateKinds.Curiousity, StateKinds.Patrol, };
    public static int ConvertStateKindToInt(StateKinds kinds) => (int)kinds;
    private Human Person { get; set; }
    public HumanState(Human person) => Person = person;
    protected bool shouldOnGuard => Person.stayOnGaurd;
    protected Transform ActorTransform => Person.ActorTransform;
    protected HumanWeapon Weapon => Person.Weapon;
    protected SkillLoader_Human skillLoader => Person.skillLoader;
    protected InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState GetHoldState => Person.GetHoldingState;
    protected void HandleWeapon(HumanAniState.StateKind stateKind)
        => Person.HoldWeapon(stateKind == HumanAniState.StateKind.HoldingWeapon || stateKind == HumanAniState.StateKind.UsingWeapon,
                                stateKind == HumanAniState.StateKind.HoldingWeapon ? InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState.Holding : InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState.Using);
    new public PersonPrepareData prepareData { set { base.prepareData = value; } get { return base.prepareData as PersonPrepareData; } }
    protected HumanStateModuleHandler ModuleHandler { get { return Person.ModuleHandler; } }
    protected bool stateLock = false;

    protected override void StartModule()
    {
        stateLock = false;
        OnStartModule();
    }

    protected abstract void OnStartModule();
    // Coroutine
    protected List<Coroutine> Coroutines { set; get; } = new List<Coroutine>();
    public Coroutine StartCoroutine(IEnumerator doFunction)
    {
        var coroutine = Person.StartCoroutine(doFunction);
        Coroutines.Add(coroutine);
        return coroutine;
    }
    protected void StopAllCoroutine() => Coroutines.ForEach(x => { if (x != null) Person.StopCoroutine(x); });

    // APH
    public AnimationPointHandler GetNewAPH(int APCounts, AnimationPointHandler.WalkingState walkingState = AnimationPointHandler.WalkingState.Walk)
    {
        return APHManager.Instance.GetNewAPH<HumanAnimationPoint>(Person.APHGroup, APCounts, walkingState);
    }
    protected void SetAPsImmediate(AnimationPoint ap, HumanAniState.StateKind kind, float time)
    {
        var dir = ActorTransform.position + ActorTransform.forward;
        SetAPs(ap, dir, kind, time, false, true);
    }
    protected void SetAPs(AnimationPoint ap, Transform target, HumanAniState.StateKind kind, float time, bool shouldReachTargetPosition, bool shouldLookAtTarget)
        => SetAPs(ap, target.position, kind, time, shouldReachTargetPosition, shouldLookAtTarget, target);
    protected void SetAPs(AnimationPoint ap, Vector3 target, HumanAniState.StateKind kind, float time, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false, Transform targetTransform = null)
    {
        ap.animationPointData.InteractionObj = Weapon;
        var canYield = CanYeildList.Contains(ModuleHandler.GetPlayingModuleStateKind());
        ap.SetAP(Person.ActorTransform.position, target, (int)kind, time, canYield, shouldReachTargetPosition, shouldLookAtTarget, canYield ? null : targetTransform);
    }
    protected void SetAPH(AnimationPointHandler aph = null, bool needFuncAfterAPH = false)
    {
        Person.SetAPH(aph, needFuncAfterAPH ? AfterAPHDone : null);
    }
    protected virtual void AfterAPHDone() { }
    public void SetState(StateKinds kinds, PersonPrepareData prepareData)
    {
        // Adding a delay to prevent multiple state changes within a single frame.
        if (!stateLock)
        {
            TimeCounter.Instance.SetTimeCounting(Time.fixedDeltaTime, () => { SetStateToPerson(kinds, prepareData); });
        }

        stateLock = true;
    }
    private void SetStateToPerson(StateKinds kinds, PersonPrepareData prepareData)
    {
        Person.HumanInfoUI.StateModule.text = "before : " + Person.ModuleHandler.GetPlayingModuleStateKind().ToString() + "\nNow :" + kinds.ToString();
        Person.SetState(ConvertStateKindToInt(kinds), prepareData);
    }
    public void SetNormalState() => SetState(StateKinds.Normal, null);
    protected bool IsTargetModelSame(HumanState stateModule)
    {
        return prepareData.target == stateModule.prepareData.target;
    }

    // Sight
    protected bool IsInSight(Transform target)
        => Person.IsHitToTarget(target);
    public void StartTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop)
    {
        var playingModule = ModuleHandler.GetPlayingModule();
        var playingKind = ModuleHandler.GetThisKind(playingModule);
        var thisKind = ModuleHandler.GetThisKind(this);
        StartCoroutine(Person.DoTracingTargetInSight(target, () => conditionOfEndLoop() && playingKind == thisKind, ShouldStopAfterCast));
    }
    public IEnumerator DoTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Func<bool, bool> ShouldStopAfterCast)
    {
        var maxTime = 600f;
        var time = 0f;
        var loopTime = 0.1f;
        while (time < maxTime && !conditionOfEndLoop())
        {
            var isInSight = IsInSight(target);
            if (ShouldStopAfterCast.Invoke(isInSight))
            {
                yield break;
            }

            time += loopTime;
            yield return new WaitForSeconds(loopTime);
        }

        if (!conditionOfEndLoop())
            ShouldStopAfterCast?.Invoke(false);

        if (time > maxTime)
            Debug.Log("DoTracingTargetInSight closed by force : its over than " + maxTime + "sec.\n" + "instanceID : " + ActorTransform.GetInstanceID());

        yield break;
    }

    public override void Exit()
    {
        prepareData = null;
        StopAllCoroutine();
    }

    protected virtual bool ShouldStopAfterCast(bool isHit) { return false; }
    public static List<StateModule> GetStatesList(Human person)
    {
        if (person is Human)
        {
            var list = new List<StateModule>();
            for (StateKinds kinds = StateKinds.Normal; kinds != StateKinds.Non; kinds++)
            {
                switch (kinds)
                {
                    case StateKinds.Normal: list.Add(new Normal_HumanState(person)); break;
                    case StateKinds.Sensed: list.Add(new Sensed_HumanState(person)); break;
                    case StateKinds.Curiousity: list.Add(new Curiousity_HumanState(person)); break;
                    case StateKinds.Tracking: list.Add(new Tracking_HumanState(person)); break;
                    case StateKinds.Patrol: list.Add(new Patrol_HumanState(person)); break;
                    case StateKinds.Attack: list.Add(new Attack_HumanState(person)); break;
                    case StateKinds.Dead: list.Add(new Dead_HumanState(person)); break;
                    default: list.Add(null); break;
                }
            }

            return list;
        }

        return null;
    }

    public class PersonPrepareData : PrepareData
    {
        public Transform target { private set; get; }
        public Vector3 lastDetectedStandPosition { set; get; }
        public PersonPrepareData(Transform target) { this.target = target; this.lastDetectedStandPosition = target.position; }
        public PersonPrepareData(Transform target, Vector3 lastStandingPosition)
        {
            this.target = target;
            this.lastDetectedStandPosition = lastStandingPosition;
        }
        public PersonPrepareData CopyTarget(PersonPrepareData personPrepareData)
        {
            return new PersonPrepareData(personPrepareData.target);
        }
    }
}
