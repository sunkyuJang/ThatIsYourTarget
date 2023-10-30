using JExtentioner;
using SensorToolkit;
using System;
using System.Collections;
using UnityEngine;

public abstract class Model : MonoBehaviour, IDamageController, IObjDetectorConnector_OnAll
{
    public enum ModelKinds { Person, Player }
    public float HP { set { HP -= value; HP = HP < 0 ? 0 : HP; } get { return HP; } }
    public int state { private set; get; } = 0;
    public Transform ActorTransform { private set; get; }

    // DMG
    float MaxAcceptableDmgTime { set; get; } = 2f;
    bool CanAcceptableDmg { set; get; } = true;

    //APH
    protected ModelAnimationPlayer ModelAnimationPlayer { set; get; }
    ModelAPHJobManger ModelAPHJobManger { set; get; }
    public Transform APHGroup { private set; get; }

    // Module
    public StateModuleHandler ModuleHandler { protected set; get; }

    // Sight
    public FOVCollider FOVCollider;
    public float SightLength { get { return FOVCollider.Length * FOVCollider.transform.lossyScale.x; } }

    // Weapon
    [SerializeField]
    private WeaponHolster weaponKeepingHolster;
    [SerializeField]
    private WeaponHolster weaponGrabHolster;
    public Weapon Weapon { get { return weaponKeepingHolster.GetWeapon(); } }

    protected virtual void Awake()
    {
        ActorTransform = transform.Find("Actor");
        ModelAnimationPlayer = new ModelAnimationPlayer(this, ActorTransform);

        APHGroup = transform.Find("APHGroup");
        ModelAPHJobManger = new ModelAPHJobManger(null, null, APHGroup, ModelAnimationPlayer);

        ModuleHandler = SetStateModuleHandler();
    }
    protected abstract StateModuleHandler SetStateModuleHandler();
    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => APHManager.Instance.IsReady);
        ModelAPHJobManger.StartJob();
    }
    public void SetState(int newState, StateModule.PrepareData prepareData = null)
    {
        ModuleHandler.EnterModule(newState, prepareData);
    }
    public void SetAPH(AnimationPointHandler handler, Action nextActionFromState = null)
    {
        ModelAPHJobManger.SetAPH(handler, nextActionFromState);
        ModelAPHJobManger.StartJob();
    }

    public Coroutine TracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Func<bool, bool> ShouldStopAfterCast)
    {
        return StartCoroutine(DoTracingTargetInSight(target, conditionOfEndLoop, ShouldStopAfterCast));
    }
    protected IEnumerator DoTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Func<bool, bool> ShouldStopAfterCast)
    {
        var maxTime = 600f;
        var time = 0f;
        while (time < maxTime && !conditionOfEndLoop())
        {
            var isHit = IsHitToTarget(target, SightLength);
            if (ShouldStopAfterCast.Invoke(isHit))
            {
                yield break;
            }

            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (!conditionOfEndLoop())
            ShouldStopAfterCast?.Invoke(false);

        if (time > maxTime)
            Debug.Log("DoTracingTargetInSight closed by force : its over than " + maxTime + "sec.\n" + "instanceID : " + transform.GetInstanceID());

        yield break;
    }
    public bool IsHitToTarget(Transform target, float dist = 0f) => FOVCollider.transform.IsRayHitToTarget(target, dist);
    public void OnContecting(ObjDetector detector, Collider collider) { if (IsHitToTarget(collider.transform)) OnContecting(collider); }
    public void OnDetected(ObjDetector detector, Collider collider) { if (IsHitToTarget(collider.transform)) OnDetected(collider); }
    public void OnRemoved(ObjDetector detector, Collider collider) { if (IsHitToTarget(collider.transform)) OnRemoved(collider); }
    public virtual void OnContecting(Collider collider) { }
    public virtual void OnDetected(Collider collider) { }
    public virtual void OnRemoved(Collider collider) { }
    protected virtual void DoDie() { }
    public bool SetDamage(float damege)
    {
        if (CanAcceptableDmg)
        {
            CanAcceptableDmg = false;
            Action removeTimeData = () => { CanAcceptableDmg = true; };
            TimeCounter.Instance.SetTimeCounting(MaxAcceptableDmgTime, removeTimeData);

            HP = damege;

            if (HP <= 0)
            {
                DoDie();
            }
        }

        return false;
    }

    public void HoldWeapon(bool shouldHold)
    {
        var from = shouldHold ? weaponKeepingHolster : weaponGrabHolster;
        var to = shouldHold ? weaponGrabHolster : weaponKeepingHolster;

        to.HoldWeapon(from.GetWeapon());
    }
}
