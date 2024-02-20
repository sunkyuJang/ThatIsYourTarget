using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PersonConversationHandler : ConversationHandler
{
    new HumanStateModuleHandler ModuleHandler { get { return base.ModuleHandler as HumanStateModuleHandler; } }
    public PersonConversationHandler(Model model) : base(model) { }
    protected override bool OnCanEnterConversation()
    {
        var state = ModuleHandler.GetPlayingModuleStateKind();
        return state == HumanState.StateKinds.Normal;
    }

    protected override void OnPrepaerConversation()
    {
        PlayAPH();
    }

    protected override void OnStartConversation()
    {
        PlayAPH();
    }

    protected override void OnEndConversation()
    {
        SetAPH();
    }

    protected void WhenSuddenEndedConversation()
    {
        var conversationState = GetState(ModuleHandler.GetPlayingModuleStateKind());

        if (conversationState == ConversationEntry.SuddenEndedState.UnHold)
            AlertAPHDone?.Invoke(this);

        AlertSuddenEnded?.Invoke(this, conversationState);
    }

    protected void PlayAPH()
    {
        var copiedAPH = APHManager.Instance.GetCopyAPH<HumanAnimationPoint>(PlayingAPH);
        SetAPH(copiedAPH, () => { WhenAPHDone(copiedAPH); });
        ModuleHandler.SetWhenStateChange(WhenSuddenEndedConversation);
    }

    protected void WhenAPHDone(AnimationPointHandler copiedAPH = null)
    {
        if (copiedAPH == null || copiedAPH.isAPHDone)
        {
            AlertAPHDone?.Invoke(this);
        }
        else
        {
            var originalAPH = PlayingAPH == OriginalWaitingAPH ? OriginalWaitingAPH : OriginalStartAPH;
            originalAPH.index = copiedAPH.index;
        }
    }

    private ConversationEntry.SuddenEndedState GetState(HumanState.StateKinds personState)
    {
        switch (personState)
        {
            case HumanState.StateKinds.Dead: return ConversationEntry.SuddenEndedState.No_Response;
            case HumanState.StateKinds.Attack: return ConversationEntry.SuddenEndedState.Combat;
            case HumanState.StateKinds.Curiousity: return ConversationEntry.SuddenEndedState.Hold;
            case HumanState.StateKinds.Normal: return ConversationEntry.SuddenEndedState.UnHold;
        }

        return ConversationEntry.SuddenEndedState.Non;
    }
    protected override void OnAlertHold()
    {
        var aph = GetNewAnipointHandler<HumanAnimationPoint>(1, AnimationPointHandler.WalkingState.Walk);
        aph.shouldLoop = true;
        var ap = aph.GetAnimationPoint<HumanAnimationPoint>(1);
        SetAPImmidiate(ap, (int)HumanAniState.StateKind.Standing, -1f);
        SetAPH(aph);
    }

    protected override void OnAlertUnHold()
    {
        WhenAPHDone();
    }

    protected override void OnAlertCombat(Vector3 targetPosition)
    {

    }

    protected override void OnAlertNonResponce(Vector3 targetPosition)
    {
        var aph = GetNewAnipointHandler<HumanAnimationPoint>(1, AnimationPointHandler.WalkingState.Walk);
        var ap = aph.GetAnimationPoint<HumanAnimationPoint>(1);
        SetAP(ap, targetPosition, (int)HumanAniState.StateKind.LookAround);
        SetAPH(aph);
    }
}
