using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Person : Model
{
    enum StateByDist { Notice = 3, Attack = 1 }
    [SerializeField]
    private Renderer modelRenderer;
    private PersonState currentState = null;
    internal object idmgController;

    public Weapon weapon { private set; get; } = null;
    protected override StateModuleHandler SetStateModuleHandler()
    {
        return new PersonStateMouleHandler(this);
    }
    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        SetState(PersonState.ConvertStateKindToInt(PersonState.StateKinds.Normal));
        yield return null;
    }
    public Material belongTo
    {
        set { modelRenderer.material = value; }
        get { return modelRenderer.material; }
    }

    bool ShouldRecongnize(Transform target) => target.GetComponent<Player>()?.belongTo == belongTo;

    public ActionPointHandler GetNewAPH(int APCounts, ActionPointHandler.WalkingState walkingState = ActionPointHandler.WalkingState.Walk)
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

    public void SetAPs(ActionPoint ap, Transform target, PersonAniState.StateKind kind, bool isTimeFixed = false, float time = 0, bool shouldChangePosition = false, bool shouldChangeRotation = false)
    {
        if (isTimeFixed)
        {
            ap.SetAPWithFixedDuring(modelHandler.transform, target, (int)kind, kind.ToString(), shouldChangePosition, shouldChangeRotation);
        }
        else
        {
            ap.SetAPWithDuring(modelHandler.transform, target, (int)kind, time, shouldChangePosition, shouldChangeRotation);
        }
    }

    public override void OnContecting(Collider collider)
    {
        SetSensedState(collider, true);
    }

    public override void OnRemoved(Collider collider)
    {
        SetSensedState(collider, false);
    }

    void SetSensedState(Collider collider, bool isContected)
    {
        var sensedIndex = PersonState.ConvertStateKindToInt(PersonState.StateKinds.Sensed);
        var state = moduleHandler.GetModule(sensedIndex);
        if (state != null)
        {
            if (state is Sensed_PersonState)
            {
                (state as Sensed_PersonState)?.PrepareState(new Sensed_PersonState.PreparingData(collider.transform, isContected));
                SetState(sensedIndex);
            }
        }
    }

    protected override void DoDie()
    {
        SetState((int)PersonState.StateKinds.Dead);
    }
}
