using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public abstract class ConversationHandler : IConversationSequence
{
    private Model Model { set; get; }
    protected void SetAPH(AnimationPointHandler aph = null, Action whenAPHDone = null) => Model.SetAPH(aph, whenAPHDone);
    protected Coroutine StartCoroutine(IEnumerator enumerator) => Model.StartCoroutine(enumerator);
    protected StateModuleHandler ModuleHandler { get { return Model.ModuleHandler; } }
    protected AnimationPointHandler OriginalWaitingAPH { set; get; }
    protected AnimationPointHandler OriginalStartAPH { set; get; }
    protected AnimationPointHandler PlayingAPH { set; get; }
    protected Action<IConversationSequence> AlertAPHDone { set; get; }
    protected Action<IConversationSequence, ConversationEntry.SuddenEndedState> AlertSuddenEnded { set; get; }
    public ConversationHandler(Model model)
    {
        Model = model;
    }
    public bool CanEnterConversation()
    {
        return OnCanEnterConversation();
    }

    public void PrepaerConversation(AnimationPointHandler waitingAPH, AnimationPointHandler startAPH, Action<IConversationSequence> alertAPHDone, Action<IConversationSequence, ConversationEntry.SuddenEndedState> alertsuddenEnded)
    {
        OriginalWaitingAPH = waitingAPH;
        OriginalStartAPH = startAPH;
        AlertAPHDone = alertAPHDone;
        AlertSuddenEnded = alertsuddenEnded;

        PlayingAPH = OriginalWaitingAPH;
        OnPrepaerConversation();
    }

    public void StartConversation()
    {
        PlayingAPH = OriginalStartAPH;
        OnStartConversation();
    }

    public void EndConversation()
    {
        OnEndConversation();
    }

    public void AlertHold()
    {
        OnAlertHold();
    }

    public void AlertUnHold()
    {
        OnAlertUnHold();
    }

    public void AlertCombat(Vector3 targetPosition)
    {
        OnAlertCombat(targetPosition);
    }

    public void AlertNonResponce(Vector3 targetPosition)
    {
        OnAlertNonResponce(targetPosition);
    }

    protected AnimationPointHandler GetNewAnipointHandler<T>(int count, AnimationPointHandler.WalkingState walkingState) where T : AnimationPoint
    {
        return null;
        var aph = APHManager.Instance.GetNewAPH<T>(Model.APHGroup, count, walkingState);
        return aph;
    }

    protected void SetAPImmidiate(AnimationPoint ap, int state, float time = 0f)
    {
        // var dir = Model.ActorTransform.position + Model.ActorTransform.forward;
        // ap.SetAP(Model.ActorTransform.position, dir, state, time, false, false);
    }
    protected void SetAP(AnimationPoint ap, Vector3 to, int state, float time = 0f, bool shouldChangePosition = false, bool shouldChangeRotation = false)
    {
        // ap.SetAP(Model.ActorTransform.position, to, state, time, shouldChangePosition, shouldChangeRotation);
    }

    protected abstract bool OnCanEnterConversation();
    protected abstract void OnPrepaerConversation();
    protected abstract void OnStartConversation();
    protected abstract void OnEndConversation();
    protected abstract void OnAlertHold();
    protected abstract void OnAlertUnHold();
    protected abstract void OnAlertCombat(Vector3 targetPosition);
    protected abstract void OnAlertNonResponce(Vector3 targetPosition);
}
