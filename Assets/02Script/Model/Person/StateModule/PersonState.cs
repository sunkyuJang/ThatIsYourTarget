using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class PersonState : StateModule
{
    public enum StateKinds
    {
        Normal,
        Sensed,
        Curiousity,
        //Warn,
        //Follow,
        //Wait,
        DrawWeapon,
        Tracking,
        Patrol,
        Attack,
        Hit,
        //Avoid,
        Dead,
        Non
    }
    public static int ConvertStateKindToInt(StateKinds kinds) => (int)kinds;
    private Person Person { get; set; }
    public PersonState(Person person) => Person = person;
    protected Transform ActorTransform { get { return Person.ActorTransform; } }
    protected Transform FovTransform { get { return Person.FOVCollider.transform; } }
    protected PersonWeapon Weapon { get { return Person.Weapon; } }
    protected void HoldWeapon(bool shouldHold) => Person.HoldWeapon(shouldHold);
    new public PersonPrepareData prepareData { set { base.prepareData = value; } get { return base.prepareData as PersonPrepareData; } }
    protected PersonStateModuleHandler ModuleHandler { get { return Person.ModuleHandler; } }
    protected List<Coroutine> Coroutines { set; get; } = new List<Coroutine>();
    public Coroutine StartCoroutine(IEnumerator doFunction)
    {
        var coroutine = Person.StartCoroutine(doFunction);
        Coroutines.Add(coroutine);
        return coroutine;
    }
    protected void StopAllCoroutine() => Coroutines.ForEach(x => { if (x != null) Person.StopCoroutine(x); });
    // APH
    protected AnimationPointHandler GetNewAPH(int APCounts, AnimationPointHandler.WalkingState walkingState = AnimationPointHandler.WalkingState.Walk)
    {
        var requireAPCount = APCounts;
        var apPooler = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP);
        var APs = new List<AnimationPoint>();
        APs.Capacity = requireAPCount;

        for (int i = 0; i < requireAPCount; i++)
        {
            var ap = apPooler.GetNewOne<PersonAnimationPoint>();
            ap.gameObject.SetActive(true);
            APs.Add(ap);
        }

        var aph = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.APH).GetNewOne<AnimationPointHandler>();
        aph.transform.SetParent(Person.APHGroup);
        aph.gameObject.SetActive(true);
        aph.SetAPs(APs);
        aph.shouldLoop = false;
        aph.walkingState = walkingState;
        return aph;
    }
    protected void SetAPs(AnimationPoint ap, Transform target, PersonAniState.StateKind kind, float time = 0, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        SetAPs(ap, target.position, kind, time, shouldReachTargetPosition, shouldLookAtTarget);
    }
    protected void SetAPsImmediate(AnimationPoint ap, PersonAniState.StateKind kind, float time = 0)
    {
        var dir = ActorTransform.position + ActorTransform.forward;
        SetAPs(ap, dir, kind, time, false, true);
    }
    protected void SetAPs(AnimationPoint ap, Vector3 target, PersonAniState.StateKind kind, float time = 0, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        if (ap is PersonAnimationPoint)
        {
            (ap as PersonAnimationPoint).SetAP(Person.ActorTransform.position, target, kind, time, shouldReachTargetPosition, shouldLookAtTarget);
        }
    }
    protected void SetAPH(AnimationPointHandler aph = null, bool needFuncAfterAPH = false)
    {
        Person.SetAPH(aph, needFuncAfterAPH ? AfterAPHDone : null);
    }
    protected virtual void AfterAPHDone() { }

    // State
    public void SetState(StateKinds kinds, PersonPrepareData prepareData)
    {
        Person.personInfoUI.StateModule.text = "before : " + Person.ModuleHandler.GetPlayingModuleStateKind().ToString() + "\nNow :" + kinds.ToString();
        Person.SetState(ConvertStateKindToInt(kinds), prepareData);
    }
    public void SetNormalState() => SetState(StateKinds.Normal, null);
    protected bool IsTargetModelSame(PersonState stateModule)
    {
        return prepareData.target == stateModule.prepareData.target;
    }

    // Sight
    protected bool IsInSight(Transform target) => Person.IsHitToTarget(target);

    public void StartTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop)
    {
        var playingModule = ModuleHandler.GetPlayingModule();
        var playingKind = ModuleHandler.GetThisKind(playingModule);
        var thisKind = ModuleHandler.GetThisKind(this);
        StartCoroutine(Person.DoTracingTargetInSight(target, () => conditionOfEndLoop() && playingKind == thisKind, ShouldStopAfterCast));
    }

    public override void Exit()
    {
        prepareData = null;
        StopAllCoroutine();
    }

    protected virtual bool ShouldStopAfterCast(bool isHit) { return false; }
    public static List<StateModule> GetStatesList(Person person)
    {
        if (person is Person)
        {
            var list = new List<StateModule>();
            for (StateKinds kinds = StateKinds.Normal; kinds != StateKinds.Non; kinds++)
            {
                switch (kinds)
                {
                    case StateKinds.Normal: list.Add(new Normal_PersonState(person)); break;
                    case StateKinds.Sensed: list.Add(new Sensed_PersonState(person)); break;
                    case StateKinds.Curiousity: list.Add(new Curiousity_PersonState(person)); break;
                    case StateKinds.DrawWeapon: list.Add(new DrawWeapon_PersonState(person)); break;
                    case StateKinds.Tracking: list.Add(new Tracking_PersonState(person)); break;
                    case StateKinds.Patrol: list.Add(new Patrol_PersonState(person)); break;
                    case StateKinds.Attack: list.Add(new Attack_PersonState(person)); break;
                    case StateKinds.Dead: list.Add(new Dead_PersonState(person)); break;
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
        public PersonPrepareData(Transform target) { this.target = target; }
    }
}
