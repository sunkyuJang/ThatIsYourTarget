using System;
using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public abstract class AnimationPoint : MonoBehaviour
{
    public AnimatorController animatorController;
    public virtual bool HasAction { get { return true; } }
    [HideInInspector]
    protected int State { set; get; } = 0;
    [HideInInspector]
    public float during = 0;
    [HideInInspector]
    public float targetDegree = 0;
    public Action<string> EventTrigger { set; get; }
    public Action whenAnimationStart { set; get; }
    public Action whenAnimationEnd { set; get; }
    public bool IsSameState(AnimationPoint other)
    {
        return other.State == State;
    }
    public ChildAnimatorState GetState(string stateName)
    {
        var aniState = animatorController.layers;
        foreach (AnimatorControllerLayer layer in aniState)
        {
            foreach (ChildAnimatorState state in layer.stateMachine.states)
            {
                if (state.state.name == stateName)
                    return state;
            }

            foreach (ChildAnimatorStateMachine machine in layer.stateMachine.stateMachines)
            {
                foreach (ChildAnimatorState state in machine.stateMachine.states)
                {
                    if (state.state.name == stateName)
                    {
                        return state;
                    }
                }
            }
        }

        return new ChildAnimatorState();
    }
    public AnimationClip GetAnimationClip(ChildAnimatorState state)
    {
        if (state.state != null)
        {
            var motionName = state.state.motion.name;
            var clips = animatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == motionName)
                {
                    return clip;
                }
            }
        }
        return null;
    }
    public float GetAnimationClipLength(string stateName)
    {
        var state = GetState(stateName);
        var clip = GetAnimationClip(state);
        return clip == null ? -1f : clip.length / state.state.speed;
    }
    public AnimationEvent[] GetAnimationEvent(string stateName)
    {
        var state = GetState(stateName);
        var clip = GetAnimationClip(state);
        return clip == null ? null : clip.events;
    }
    private void ChangePosition(Vector3 position)
    {
        transform.position = position;
    }
    private void MakeLookAtTo(Vector3 to) => transform.LookAt(to - (Vector3.up * to.y + Vector3.up * transform.position.y));
    private void SetPositionForTracking(Vector3 from, Vector3 to, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        ChangePosition(from);
        MakeLookAtTo(shouldLookAtTarget ? to : from);
        if (shouldReachTargetPosition)
        {
            ChangePosition(to);
        }
    }

    protected void SetAPWithDuring(Vector3 from, Vector3 to, int state, float time, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        this.State = state;
        during = time;
        SetPositionForTracking(from, to, shouldReachTargetPosition, shouldLookAtTarget);
    }

    protected void SetAPWithFixedDuring(Vector3 from, Vector3 to, int state, string kind, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        if (kind == null) Debug.Log("the animation fixed kind is null");
        SetAPWithDuring(from, to, state, GetAnimationClipLength(kind), shouldReachTargetPosition, shouldLookAtTarget);
    }

    public void SetAP(Vector3 from, Vector3 to, int state, float time, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
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
}

