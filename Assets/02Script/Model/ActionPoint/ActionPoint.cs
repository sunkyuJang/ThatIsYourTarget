using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public class ActionPoint : MonoBehaviour
{
    public AnimatorController animatorController;
    public virtual bool HasAction { get { return true; } }
    [HideInInspector]
    public int state { protected set; get; } = 0;
    [HideInInspector]
    public float during = 0;
    [HideInInspector]
    public float targetDegree = 0;
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
    public float GetLength(string stateName)
    {
        var state = GetState(stateName);
        if (state.state != null)
        {
            var speed = state.state.speed;
            var motionName = state.state.motion.name;
            var clips = animatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == motionName)
                {
                    return clip.length / speed;
                }
            }
        }

        return 0;
    }
    public void ChangePosition(Vector3 position) => transform.position = position;
    public void MakeLookAtTo(Vector3 to) => transform.LookAt(to - Vector3.up * to.y);
    public void SetPositionForTracking(Transform from, Transform to, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        ChangePosition(from.position);
        MakeLookAtTo(shouldLookAtTarget ? to.position : from.forward);
        if (shouldReachTargetPosition)
            ChangePosition(to.position);
    }

    public void SetAPWithDuring(Transform from, Transform to, int state, float time, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        this.state = state;
        during = time;
        SetPositionForTracking(from, to, shouldReachTargetPosition, shouldLookAtTarget);
    }

    public void SetAPWithFixedDuring(Transform from, Transform to, int state, string kind, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        SetAPWithDuring(from, to, state, GetLength(kind), shouldReachTargetPosition, shouldLookAtTarget);
    }
}

