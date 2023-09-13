using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        PrepareAttack,
        Tracking,
        Patrol,
        Hit,
        //Avoid,
        Dead,
        Non
    }
    public static int ConvertStateKindToInt(StateKinds kinds) => (int)kinds;
    private Person Person { get; set; }
    public PersonState(Person person) => Person = person;
    protected Transform ActorTransform { get { return Person.ActorTransform; } }
    protected PersonWeapon Weapon { get { return Person.Weapon; } }
    protected void HoldWeapon(bool shouldHold) => Person.HoldWeapon(shouldHold);
    new public PersonPrepareData prepareData { set { base.prepareData = value; } get { return base.prepareData as PersonPrepareData; } }
    protected PersonStateModuleHandler ModuleHandler { get { return Person.ModuleHandler; } }
    protected Coroutine StartCoroutine(IEnumerator doFunction) { return Person.StartCoroutine(doFunction); }

    // APH
    protected AnimationPointHandler GetNewAPH(int APCounts, AnimationPointHandler.WalkingState walkingState = AnimationPointHandler.WalkingState.Walk, PersonAniState.StateKind kind = PersonAniState.StateKind.Non)
    {
        var requireAPCount = APCounts;
        var apPooler = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP);
        var APs = new List<AnimationPoint>();
        APs.Capacity = requireAPCount;

        for (int i = 0; i < requireAPCount; i++)
            APs.Add(apPooler.GetNewOne<PersonAnimationPoint>());

        var aph = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.APH).GetNewOne<AnimationPointHandler>();
        aph.SetAPs(APs);
        aph.shouldLoop = false;
        aph.walkingState = walkingState;
        return aph;
    }
    protected void SetAPs(AnimationPoint ap, Transform target, PersonAniState.StateKind kind, float time = 0, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        SetAPs(ap, target.position, kind, time, shouldReachTargetPosition, shouldLookAtTarget);
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
        Person.SetAPH(aph, needFuncAfterAPH ? () => AfterAPHDone(out PersonPrepareData data) : null);
    }
    protected virtual StateKinds AfterAPHDone(out PersonPrepareData data)
    {
        data = null;
        return StateKinds.Normal;
    }

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
    protected bool IsInSight(Transform target) => Person.IsInSight(target);
    public Coroutine TracingTargetInSightProcess(Transform target, Func<bool> conditionOfEndLoop) => Person.TracingTargetInSight(target, conditionOfEndLoop, WhenTargetInSight);

    public override void Exit()
    {
        prepareData = null;
    }

    protected virtual void WhenTargetInSight(bool isHit) { }
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
                    case StateKinds.PrepareAttack: list.Add(new PrepareAttack_PersonState(person)); break;
                    case StateKinds.Tracking: list.Add(new Tracking_PersonState(person)); break;
                    case StateKinds.Patrol: list.Add(new Patrol_PersonState(person)); break;
                    case StateKinds.Hit: list.Add(new Hit_PersonState(person)); break;
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
