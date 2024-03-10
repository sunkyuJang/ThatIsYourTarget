using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEditor.Animations;
using UnityEngine;

public partial class Human : Model
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
    new public HumanStateModuleHandler ModuleHandler
        => base.ModuleHandler as HumanStateModuleHandler;
    new public SkillLoader_Human skillLoader
        => base.skillLoader as SkillLoader_Human;
    protected override StateModuleHandler SetStateModuleHandler()
        => new HumanStateModuleHandler(this);
    protected override SkillLoader SetSkillLoader(AnimatorController controller)
        => new SkillLoader_Human(controller, this);
    protected override ConversationHandler SetConversationHandler()
        => new PersonConversationHandler(this);
    new public HumanWeapon Weapon { get { return base.Weapon as HumanWeapon; } }
    public HumanInfoUI HumanInfoUI;

    new private void Awake()
    {
        base.Awake();
        HP = 10;
    }
    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        SetState(HumanState.ConvertStateKindToInt(HumanState.StateKinds.Normal));

        HumanInfoUI.human = this;
        yield break;
    }
    public Material belongTo
    {
        set { modelRenderer.material = value; }
        get { return modelRenderer.material; }
    }

    bool ShouldRecongnize(Transform target) => target.GetComponent<Player>()?.belongTo == belongTo;

    public override void OnDetected(Transform target)
    {
        SetSensedState(target, true);
    }

    void SetSensedState(Transform target, bool isContected)
    {
        var stateKind = HumanState.StateKinds.Sensed;
        var state = ModuleHandler.GetModule(stateKind);
        if (state != null)
        {
            state.TryEnter(new Sensed_HumanState.SensedPrepareData(target.transform, isContected));
        }
    }

    protected override void DoDead()
    {
        var stateKind = HumanState.StateKinds.Dead;
        SetState((int)stateKind, new HumanState.PersonPrepareData(null));
    }
}
