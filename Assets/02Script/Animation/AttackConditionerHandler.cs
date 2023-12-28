using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class AttackConditionerHandler
{

    protected List<string> requireAniNodeName = new List<string>();
    protected AnimatorController controller;
    protected AttackingComboManager AttackingComboManager { set; get; } = null;
    readonly protected int ExcuteAni = -1;
    readonly protected int ProgressToNext = 0;
    static bool IsCheckedBeforeRun = false;
    public AttackConditionerHandler(AnimatorController animatorController)
    {
        requireAniNodeName = GetAniNodeName();
        controller = animatorController;
        AttackingComboManager = AnimatorStateManager.Instance.GetAttackingComboState(controller);

        CheckRequireAniNode();
    }
    protected void CheckRequireAniNode()
    {
        if (IsCheckedBeforeRun) return;
        IsCheckedBeforeRun = true;

        AttackingComboManager.CheckRequireNodeExist(requireAniNodeName);
    }

    public abstract AnimationComboStateNode GetInitiatedNode(RequireData data);
    public AnimationComboStateNode GetNextNode(RequireData requireData, AnimationComboStateNode node) => FindNextNode(requireData, node, null);
    protected abstract AnimationComboStateNode FindNextNode(RequireData requireData, AnimationComboStateNode node, Action<Animator> parametterSetter);
    protected abstract List<string> GetAniNodeName();

    public class RequireData { }
}
