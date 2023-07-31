using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Person : Model
{
    enum StateByDist { Notice = 3, Attack = 1 }
    [SerializeField]
    private Renderer modelRenderer;
    internal object idmgController;
    new public PersonStateMouleHandler moduleHandler => base.moduleHandler as PersonStateMouleHandler;
    public PersonWeapon weapon { private set; get; } = null;

    public PersonInfoUI personInfoUI;
    protected override StateModuleHandler SetStateModuleHandler()
    {
        return new PersonStateMouleHandler(this);
    }
    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        SetState(PersonState.ConvertStateKindToInt(PersonState.StateKinds.Normal));

        personInfoUI.person = this;
        yield break;
    }
    public Material belongTo
    {
        set { modelRenderer.material = value; }
        get { return modelRenderer.material; }
    }

    bool ShouldRecongnize(Transform target) => target.GetComponent<Player>()?.belongTo == belongTo;

    public ActionPointHandler GetNewAPH(int APCounts, ActionPointHandler.WalkingState walkingState = ActionPointHandler.WalkingState.Walk, PersonAniState.StateKind kind = PersonAniState.StateKind.Non)
    {
        var requireAPCount = APCounts;
        var apPooler = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP);
        var APs = new List<ActionPoint>();
        APs.Capacity = requireAPCount;

        for (int i = 0; i < requireAPCount; i++)
            APs.Add(apPooler.GetNewOne<PersonActionPoint>());

        var aph = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.APH).GetNewOne<ActionPointHandler>();
        aph.SetAPs(APs);
        aph.shouldLoop = false;
        aph.walkingState = walkingState;
        return aph;
    }
    public void SetAPs(ActionPoint ap, Transform target, PersonAniState.StateKind kind, bool isTimeFixed = false, float time = 0, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
        => SetAPs(ap, target.transform.position, kind, isTimeFixed, time, shouldReachTargetPosition, shouldLookAtTarget);

    public void SetAPs(ActionPoint ap, Vector3 target, PersonAniState.StateKind kind, bool isTimeFixed = false, float time = 0, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        if (isTimeFixed)
        {
            ap.SetAPWithFixedDuring(modelHandler.transform.position, target, (int)kind, kind.ToString(), shouldReachTargetPosition, shouldLookAtTarget);
        }
        else
        {
            ap.SetAPWithDuring(modelHandler.transform.position, target, (int)kind, time, shouldReachTargetPosition, shouldLookAtTarget);
        }
    }

    public override void OnDetected(Collider collider)
    {
        SetSensedState(collider, true);
    }

    public override void OnRemoved(Collider collider)
    {
        SetSensedState(collider, false);
    }

    void SetSensedState(Collider collider, bool isContected)
    {
        var stateKind = PersonState.StateKinds.Sensed;
        var state = moduleHandler.GetModule(stateKind);
        if (state != null)
        {
            if (state is Sensed_PersonState)
            {
                (state as Sensed_PersonState)?.SetPrepareData(new Sensed_PersonState.PreparingData(collider.transform, isContected));
                SetState((int)stateKind);
            }
        }
    }

    protected override void DoDie()
    {
        SetState((int)PersonState.StateKinds.Dead);
    }
}
