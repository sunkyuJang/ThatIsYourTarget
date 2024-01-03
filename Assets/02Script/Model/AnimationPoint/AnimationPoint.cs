using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public abstract class AnimationPoint : MonoBehaviour
{
    public AnimatorController animatorController;
    public virtual bool HasAction { get { return true; } }
    [HideInInspector]
    public int state = 0;
    [HideInInspector]
    public float during = 0;
    public bool IsUnLimited { get { return during <= -1; } }
    [HideInInspector]
    public float targetDegree = 0;
    public Vector3 CorrectedPosition { set; get; }
    public bool ShouldcontinuousReadState { set; get; } = false;

    // event 
    public Action<int> EventTrigger { set; get; }
    public Action whenAnimationStart { set; get; }
    public Action whenAnimationEnd { set; get; }
    public Action whenAnimationExitTime { set; get; }

    // positioning
    public bool CanYield { set; get; } = true;
    public abstract bool ShouldPlaySamePosition { get; }
    public abstract void ReplaceExpectionState();

    // aiming detail
    public InteractionObj InteractionObj { set; get; } = null;
    public Weapon Weapon { get { return InteractionObj as Weapon; } }
    public Transform TargetingTarsform { set; get; } = null;
    public Transform AimTarget { set; get; } = null;

    // SkillAnimationName
    public SkillData SkillData { set; get; }

    public float GetAnimationClipLength(string stateName)
    {
        var state = AnimatorStateManager.Instance.GetStateInfo(animatorController, stateName);
        if (state == null) return -1f;
        else
            return state.Length;
    }
    public List<float> GetAnimationEvent(string stateName)
    {
        var state = AnimatorStateManager.Instance.GetStateInfo(animatorController, stateName);
        if (state == null) return null;
        else
            return state.EventsTiming;
    }

    public List<KeyValuePair<float, string>> GetExitAniEvent(string animationName)
    {
        var state = AnimatorStateManager.Instance.GetStateInfo(animatorController, animationName);
        if (state == null) return null;
        else
            return state.ExitTime;
    }
    private void ChangePosition(Vector3 position)
    {
        transform.position = position;
        CorrectedPosition = position;
    }
    private void MakeLookAtTo(Vector3 to)
    {
        transform.LookAt(to - (Vector3.up * to.y + Vector3.up * transform.position.y));
    }
    private void SetPositionForTracking(Vector3 from, Vector3 to, bool shouldReachTargetPosition, bool shouldLookAtTarget)
    {
        ChangePosition(from);
        MakeLookAtTo(shouldLookAtTarget ? to : from);
        if (shouldReachTargetPosition)
        {
            ChangePosition(to);
        }
    }

    protected void SetAPWithDuring(Vector3 from, Vector3 to, int state, float time, bool shouldReachTargetPosition, bool shouldLookAtTarget)
    {
        this.state = state;
        during = time;
        SetPositionForTracking(from, to, shouldReachTargetPosition, shouldLookAtTarget);
    }

    protected void SetAPWithFixedDuring(Vector3 from, Vector3 to, int state, string kind, bool shouldReachTargetPosition, bool shouldLookAtTarget)
    {
        if (kind == null) Debug.Log("the animation fixed kind is null");
        SetAPWithDuring(from, to, state, GetAnimationClipLength(kind), shouldReachTargetPosition, shouldLookAtTarget);
    }

    public void SetAP(Vector3 from, Vector3 to, int state, float time, bool canYield, bool shouldReachTargetPosition, bool shouldLookAtTarget, Transform targetTransform)
    {
        CanYield = canYield;
        TargetingTarsform = targetTransform;
        if (IsFixedDuring(state))
        {
            // if (IsFixedDuringInRuntime(state))
            // {
            //     var stateName = GetRuntimeStateName(state);
            //     SetAPWithFixedDuring(from, to, state, stateName, shouldReachTargetPosition, shouldLookAtTarget);
            //     return;
            // }
            SetAPWithFixedDuring(from, to, state, GetStateName(state), shouldReachTargetPosition, shouldLookAtTarget);
        }
        else
        {
            SetAPWithDuring(from, to, state, time, shouldReachTargetPosition, shouldLookAtTarget);
        }
    }
    public abstract bool IsFixedDuring(int state);
    //public abstract bool IsFixedDuringInRuntime(int state);
    public abstract string GetStateName(int state);
    public abstract string GetRuntimeStateName(int state);
}

