using JExtentioner;
using SensorToolkit;
using System;
using System.Collections;
using UnityEngine;

public abstract class Model : MonoBehaviour, IObjDetectorConnector_OnDetected, IDamagePasser
{
    public enum ModelKinds { Person, Player }
    public float HP { set; get; } = 0;
    public int state { private set; get; } = 0;
    public Transform ActorTransform;

    // DMG
    float IgnoreDmgTime { set; get; } = 2f;
    bool CanAcceptableDmg { set; get; } = true;
    [SerializeField]
    bool canDead = true;
    DamageContorller DamageContorller { set; get; }

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

    // Coversation
    public ConversationHandler ConversationHandler { protected set; get; }

    protected virtual void Awake()
    {
        ModelAnimationPlayer = new ModelAnimationPlayer(this, ActorTransform);

        DamageContorller = new DamageContorller(this, ActorTransform);

        APHGroup = transform.Find("APHGroup");
        ModelAPHJobManger = new ModelAPHJobManger(null, null, APHGroup, ModelAnimationPlayer);

        ModuleHandler = SetStateModuleHandler();

        ConversationHandler = SetConversationHandler();

        var physicalModelConnector = GetComponentInChildren<PhysicalModelConnector>();
        physicalModelConnector.SetPhysicalModelConnector(this, ConversationHandler);
    }
    protected abstract StateModuleHandler SetStateModuleHandler();
    protected abstract ConversationHandler SetConversationHandler();
    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => APHManager.Instance.IsReady);
        ModelAPHJobManger.StartJob();
    }
    public void SetState(int newState, StateModule.PrepareData prepareData = null)
    {
        ModuleHandler.EnterModule(newState, prepareData);
    }
    public void SetAPH(AnimationPointHandler handler = null, Action nextActionFromState = null)
    {
        ModelAPHJobManger.SetAPH(handler, nextActionFromState);
        ModelAPHJobManger.StartJob();
    }

    public IEnumerator DoTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Func<bool, bool> ShouldStopAfterCast)
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
    public void OnDetected(ObjDetector detector, Collider collider) { if (IsHitToTarget(collider.transform)) OnDetected(collider); }
    public virtual void OnDetected(Collider collider) { }
    protected virtual void DoDead() { }
    public void HoldWeapon(bool shouldHold)
    {
        var from = shouldHold ? weaponKeepingHolster : weaponGrabHolster;
        var to = shouldHold ? weaponGrabHolster : weaponKeepingHolster;

        to.HoldWeapon(from.GetWeapon());
    }

    public void SetDamage(object section, object parts, float damage, out bool isDead)
    {
        isDead = false;
        if (canDead)
        {
            if (CanAcceptableDmg)
            {
                CanAcceptableDmg = false;
                Action removeTimeData = () => { CanAcceptableDmg = true; };
                TimeCounter.Instance.SetTimeCounting(IgnoreDmgTime, removeTimeData);

                HP -= damage;

                if (HP <= 0)
                {
                    isDead = true;
                    DoDead();
                }
            }
        }
    }
}