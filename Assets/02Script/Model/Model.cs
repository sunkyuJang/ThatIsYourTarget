using JExtentioner;
using SensorToolkit;
using System;
using System.Collections;
using UnityEditor.Animations;
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
    [SerializeField] bool canDead = true;
    DamageContorller DamageContorller { set; get; }

    //APH
    [SerializeField] public bool stayOnGaurd = false;
    protected ModelAnimationPlayer ModelAnimationPlayer { set; get; }
    ModelAPHJobManger ModelAPHJobManger { set; get; }
    public Transform APHGroup { private set; get; }

    // Module
    public StateModuleHandler ModuleHandler { protected set; get; }

    //SkillLoader
    public SkillLoader skillLoader { private set; get; }

    // InteractionObjManager
    private InteractionObjHolsterHandler interactionManager { set; get; }
    public Weapon Weapon
    {
        get
        {
            var weapons = interactionManager.GetInteractionObj<Weapon>();
            if (weapons == null) return null;
            else return weapons[0];
        }
    }

    // Coversation
    public ConversationHandler ConversationHandler { protected set; get; }

    // Sight
    public FOVCollider FOVCollider;
    //public float SightLength { get { return FOVCollider.Length * FOVCollider.transform.lossyScale.x; } }


    protected virtual void Awake()
    {
        ModelAnimationPlayer = new ModelAnimationPlayer(this, ActorTransform);
        DamageContorller = new DamageContorller(this, ActorTransform);
        interactionManager = ActorTransform.GetComponent<InteractionObjHolsterHandler>();
        APHGroup = transform.Find("APHGroup");
        ModelAPHJobManger = new ModelAPHJobManger(this, null, null, APHGroup, ModelAnimationPlayer);
        ModuleHandler = SetStateModuleHandler();
        skillLoader = SetSkillLoader(ActorTransform.GetComponent<Animator>().runtimeAnimatorController as AnimatorController);
        //ConversationHandler = SetConversationHandler();
        var physicalModelConnector = GetComponentInChildren<PhysicalModelConnector>();
        physicalModelConnector.SetPhysicalModelConnector(this, ConversationHandler);
        FOVCollider = GetComponentInChildren<FOVCollider>();
    }
    protected abstract StateModuleHandler SetStateModuleHandler();
    protected abstract SkillLoader SetSkillLoader(AnimatorController controller);
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
    public void OnDetected(ObjDetector detector, Collider collider) => OnDetected(collider);

    public abstract void OnDetected(Collider collider); // use onDetected as for detection sensedState. onRemove is not working by some animation.
    protected virtual void DoDead() { }
    public void HoldWeapon(bool shouldHold, InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState grabbingState)
    {
        if (shouldHold)
        {
            if (Weapon is HumanWeapon)
                interactionManager.SetHold(Weapon, grabbingState, (Weapon as HumanWeapon).humanFingerPositioner.LFingerPositioner, (Weapon as HumanWeapon).humanFingerPositioner.RFingerPositioner);
            else
                interactionManager.SetHold(Weapon, grabbingState, null, null);
        }
        else
        {
            interactionManager.SetKeep(Weapon);
        }
    }

    public InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState GetHoldingState => interactionManager.GetHoldingState();

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

    // sight
    public IEnumerator DoTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Func<bool, bool> ShouldStopAfterCast)
    {
        var maxTime = 600f;
        var time = 0f;
        while (time < maxTime && !conditionOfEndLoop())
        {
            var isHit = IsHitToTarget(target);
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
    //public bool IsHitToTarget(Transform target, float dist = 0f) => FOVCollider.transform.IsRayHitToTarget(target, dist, FOVCollider.FOVAngle);
    public bool IsHitToTarget(Transform target) => FOVCollider.transform.IsRayHitToTarget(target, Vector3.Distance(target.position, FOVCollider.transform.position), FOVCollider.FOVAngle);
}