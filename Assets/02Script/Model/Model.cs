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

    // Module
    public StateModuleHandler ModuleHandler { protected set; get; }

    // Sight
    [SerializeField]
    private FOVCollider FOVCollider;
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

        var APHGroup = transform.Find("APHGroup");
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

    public bool IsInSight(Transform target) => ActorTransform.IsRayHitToTarget(target, SightLength);
    public Coroutine TracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Action<bool> whenHit)
    {
        return StartCoroutine(DoTracingTargetInSight(target, conditionOfEndLoop, whenHit));
    }
    protected IEnumerator DoTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Action<bool> whenHit)
    {
        var maxTime = 600f;
        var time = 0f;
        while (time < maxTime && !conditionOfEndLoop())
        {
            var isHit = ActorTransform.IsRayHitToTarget(target, SightLength);
            if (isHit)
            {
                whenHit?.Invoke(true);
            }

            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        whenHit?.Invoke(false);
        Debug.Log("DoTracingTargetInSight closed by force : its over than " + maxTime + "sec.\n" + "instanceID : " + transform.GetInstanceID());
        yield break;
    }
    public void OnContecting(ObjDetector detector, Collider collider) => OnContecting(collider);
    public void OnDetected(ObjDetector detector, Collider collider) => OnDetected(collider);
    public void OnRemoved(ObjDetector detector, Collider collider) => OnRemoved(collider);
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
