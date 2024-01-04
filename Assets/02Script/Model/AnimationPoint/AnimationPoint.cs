using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public abstract class AnimationPoint : MonoBehaviour
{
    public AnimatorController animatorController;
    public virtual bool HasAction { get { return true; } }
    public AnimationPointData animationPointData;
    public abstract bool ShouldPlaySamePosition { get; }
    public abstract void ReplaceExpectionState();
    public void ResetData()
    {
        animationPointData = new AnimationPointData();
    }
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
        animationPointData.CorrectedPosition = position;
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
        animationPointData.state = state;
        animationPointData.during = time;
        SetPositionForTracking(from, to, shouldReachTargetPosition, shouldLookAtTarget);
    }

    protected void SetAPWithFixedDuring(Vector3 from, Vector3 to, int state, string kind, bool shouldReachTargetPosition, bool shouldLookAtTarget)
    {
        if (kind == null) Debug.Log("the animation fixed kind is null");
        SetAPWithDuring(from, to, state, GetAnimationClipLength(kind), shouldReachTargetPosition, shouldLookAtTarget);
    }

    public void SetAP(Vector3 from, Vector3 to, int state, float time, bool canYield, bool shouldReachTargetPosition, bool shouldLookAtTarget, Transform targetTransform)
    {
        animationPointData.CanYield = canYield;
        animationPointData.TargetingTarsform = targetTransform;
        if (IsFixedDuring(state))
        {
            SetAPWithFixedDuring(from, to, state, GetStateName(state), shouldReachTargetPosition, shouldLookAtTarget);
        }
        else
        {
            SetAPWithDuring(from, to, state, time, shouldReachTargetPosition, shouldLookAtTarget);
        }
    }
    public abstract bool IsFixedDuring(int state);
    public abstract string GetStateName(int state);
    public abstract string GetRuntimeStateName(int state);

    public void ResetObj()
    {
        ResetData();
    }
}

