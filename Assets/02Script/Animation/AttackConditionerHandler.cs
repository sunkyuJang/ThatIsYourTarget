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
    protected AttackingAnimationStateManager AttackingAniStateManager { set; get; } = null;
    readonly protected int ExcuteAni = -1;
    readonly protected int ProgressToNext = 0;
    static bool IsCheckedBeforeRun = false;
    public AttackConditionerHandler(AnimatorController animatorController)
    {
        requireAniNodeName = GetAniNodeName();
        controller = animatorController;
        AttackingAniStateManager = AnimatorStateManager.Instance.GetAttackingStateManager(controller);

        CheckRequireAniNode();
    }
    protected void CheckRequireAniNode()
    {
        if (IsCheckedBeforeRun) return;
        IsCheckedBeforeRun = true;

        AttackingAniStateManager.CheckRequireNodeExist(requireAniNodeName);
    }

    public Action<Animator> GetAllTransitionPath(string Name)
    {
        var node = AttackingAniStateManager.GetStateNode(Name);
        Action<Animator> actions = (Animator animator) => { };
        GetBeforePath(ref actions, node);
        return actions;
    }

    void GetBeforePath(ref Action<Animator> actions, AnimationStateNode node)
    {
        var beforeNode = AttackingAniStateManager.GetStateNode(node.beforeAnimation);
        var count = 0;
        foreach (var nextAniString in beforeNode.nextAnimations)
        {
            if (nextAniString == node.nowAnimation)
            {
                actions += (Animator animator) => { animator.SetInteger(beforeNode.nowAnimation, count); };
                break;
            }
            count++;
        }

        if (beforeNode.beforeAnimation != "")
        {
            GetBeforePath(ref actions, beforeNode);
        }
    }
    public abstract AnimationStateNode GetInitiatedNode(RequireData data);
    public AnimationStateNode GetNextNode(RequireData requireData, AnimationStateNode node) => FindNextNode(requireData, node, null);
    protected abstract AnimationStateNode FindNextNode(RequireData requireData, AnimationStateNode node, Action<Animator> parametterSetter);
    protected abstract List<string> GetAniNodeName();
    public class RequireData { }
}
