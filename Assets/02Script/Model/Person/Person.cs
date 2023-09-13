using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Person : Model
{
    readonly public static List<ModelKinds> modelPriolity = new List<ModelKinds>()
    {
        ModelKinds.Person,
        ModelKinds.Player,
    };
    public static int GetPriolity(Transform transform)
    {
        for (int i = 0; i < modelPriolity.Count; i++)
        {
            var kind = modelPriolity[i];
            if (transform.CompareTag(kind.ToString()))
            {
                return i;
            }
        }
        return -1;
    }
    [SerializeField]
    private Renderer modelRenderer;

    object idmgController;
    new public PersonStateModuleHandler ModuleHandler => base.ModuleHandler as PersonStateModuleHandler;

    new public PersonWeapon Weapon { get { return base.Weapon as PersonWeapon; } }

    public PersonInfoUI personInfoUI;
    protected override StateModuleHandler SetStateModuleHandler()
    {
        return new PersonStateModuleHandler(this);
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

    public override void OnDetected(Collider collider)
    {
        SetSensedState(collider, true);
    }

    //public override void OnRemoved(Collider collider)
    //{
    //    SetSensedState(collider, false);
    //}

    void SetSensedState(Collider collider, bool isContected)
    {
        var stateKind = PersonState.StateKinds.Sensed;
        var state = ModuleHandler.GetModule(stateKind);
        if (state != null)
        {
            state.TryEnter(new Sensed_PersonState.SensedPrepareData(collider.transform, isContected));
        }
    }

    protected override void DoDie()
    {
        SetState((int)PersonState.StateKinds.Dead);
    }
}
