using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public class AnimationPoint : MonoBehaviour
{
    public AnimatorController animatorController;
    public virtual bool HasAction { get { return true; } }
    [HideInInspector]
    protected int state { set; get; } = 0;
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
    public void SetPositionForTracking(Vector3 from, Vector3 to, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        ChangePosition(from);
        MakeLookAtTo(shouldLookAtTarget ? to : from);
        if (shouldReachTargetPosition)
            ChangePosition(to);
    }

    protected void SetAPWithDuring(Vector3 from, Vector3 to, int state, float time, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        this.state = state;
        during = time;
        SetPositionForTracking(from, to, shouldReachTargetPosition, shouldLookAtTarget);
    }

    protected void SetAPWithFixedDuring(Vector3 from, Vector3 to, int state, string kind, bool shouldReachTargetPosition = false, bool shouldLookAtTarget = false)
    {
        SetAPWithDuring(from, to, state, GetLength(kind), shouldReachTargetPosition, shouldLookAtTarget);
    }
}

