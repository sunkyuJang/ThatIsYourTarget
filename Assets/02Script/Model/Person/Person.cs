using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMath;

public partial class Person : Model
{
    enum StateByDist { Notice = 3, Attack = 1 }
    [SerializeField]
    private Renderer modelRenderer;
    Dictionary<PersonState.StateKinds, PersonState> states { set; get; } = new Dictionary<PersonState.StateKinds, PersonState>();
    public PersonState GetState(PersonState.StateKinds state) => states != null && states.ContainsKey(state) ? states[state] : null;
    private PersonState currentState = null;
    internal object idmgController;

    public Weapon weapon { private set; get; } = null;
    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        states = PersonState.GetNewStateList(this);
        yield return null;
    }
    public Material belongTo
    {
        set { modelRenderer.material = value; }
        get { return modelRenderer.material; }
    }

    bool ShouldRecongnize(Transform target) => target.GetComponent<Player>()?.belongTo == belongTo;

    protected override void ChangedState()
    {
        if (state < states.Count)
        {
            currentState?.Exit();
            currentState = states[(PersonState.StateKinds)state];

            if (currentState == null)
            {
                Debug.Log("state dosnt exist");
            }
            else
            {
                if (currentState.IsReadyForEnter())
                {
                    currentState.Enter();
                }
                else
                {
                    currentState.EnterToException();
                }
            }
        }
    }

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

    public override void Contected(Collider collider)
    {
        SetSensedState(collider, true);
    }

    public override void Removed(Collider collider)
    {
        SetSensedState(collider, false);
    }

    void SetSensedState(Collider collider, bool isContected)
    {
        if (states.ContainsKey(PersonState.StateKinds.Sensed))
        {
            var sensedState = states[PersonState.StateKinds.Sensed];
            if (sensedState is Sensed_PersonState)
            {
                (sensedState as Sensed_PersonState)?.PrepareState(new Sensed_PersonState.PreparingData(collider.transform, isContected));
            }
        }
    }

    protected override void DoDie()
    {
        SetState((int)PersonState.StateKinds.Dead);
    }
}
