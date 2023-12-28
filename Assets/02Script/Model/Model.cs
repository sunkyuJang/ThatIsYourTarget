using JExtentioner;
using SensorToolkit;
using System;
using System.Collections;
using System.Security.Policy;
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

    protected virtual void Awake()
    {
        ModelAnimationPlayer = new ModelAnimationPlayer(this, ActorTransform);
        DamageContorller = new DamageContorller(this, ActorTransform);
        interactionManager = ActorTransform.GetComponent<InteractionObjHolsterHandler>();
        APHGroup = transform.Find("APHGroup");
        ModelAPHJobManger = new ModelAPHJobManger(this, null, null, APHGroup, ModelAnimationPlayer);
        ModuleHandler = SetStateModuleHandler();
        //ConversationHandler = SetConversationHandler();
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
    public void OnDetected(ObjDetector detector, Collider collider) { OnDetected(collider); }
    public virtual void OnDetected(Collider collider) { }
    protected virtual void DoDead() { }
    public void HoldWeapon(bool shouldHold, InteractionObjGrabRig.State grabbingState)
    {
        if (shouldHold)
        {
            interactionManager.SetHold(Weapon, grabbingState);
        }
        else
        {
            interactionManager.SetKeep(Weapon);
        }
    }

    public InteractionObjGrabRig.State GetHoldingState => interactionManager.GetHoldingState(Weapon);

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