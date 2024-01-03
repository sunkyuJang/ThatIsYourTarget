using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering;
public class AnimatorStateListManager : MonoBehaviour
{
    public SerializedDictionary<string, AnimationStateInfo> serializerDictionary = new SerializedDictionary<string, AnimationStateInfo>();
    public void InitializeStateData(AnimatorController controller)
    {
        serializerDictionary.Clear();
        foreach (var layer in controller.layers)
        {
            ProcessStateMachine(layer.stateMachine, controller);
        }
    }

    private void ProcessStateMachine(AnimatorStateMachine stateMachine, AnimatorController controller)
    {
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            ProcessStateMachine(subStateMachine.stateMachine, controller);
        }

        foreach (var state in stateMachine.states)
        {
            List<float> events = new List<float>();
            var cilp = state.state.motion as AnimationClip;
            if (cilp != null)
            {
                foreach (var eachEvent in cilp.events)
                {
                    events.Add(eachEvent.time);
                }
            }

            float length = CalculateStateLength(state.state);
            serializerDictionary.Add(state.state.name, new AnimationStateInfo(state.state.name, length, events, state.state.transitions.ToList()));
        }
    }

    private float CalculateStateLength(AnimatorState state)
    {
        float length = 0f;
        Motion motion = state.motion;

        if (motion is AnimationClip clip)
        {
            length = clip.length / state.speed;
        }

        return length;
    }

    public AnimationStateInfo GetStateInfo(string stateName)
    {
        if (serializerDictionary.ContainsKey(stateName))
        {
            return serializerDictionary[stateName];
        }

        return null;
    }
}
