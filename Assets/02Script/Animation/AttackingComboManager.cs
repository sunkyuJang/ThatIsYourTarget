using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class AttackingComboManager : MonoBehaviour
{
    public AnimatorController Controller { set; get; }
    [SerializeField] private SerializedDictionary<string, AnimationComboStateNode> aniStateNode = new SerializedDictionary<string, AnimationComboStateNode>();
    private List<AnimationComboStateNode> LoopList = new List<AnimationComboStateNode>();
    private static string targetLayerName = "WeaponMotion";
    private static string targetSubStateName = "AttackingWeapon";

    public AnimationComboStateNode GetStartNode() => aniStateNode[targetSubStateName];

    [SerializeField] private List<string> AddiedParametaList = new List<string>();
    public void InitializeStateData(AnimatorController controller)
    {
        // set all attck state
        aniStateNode.Clear();
        LoopList.Clear();
        AddiedParametaList.Clear();
        AnimationStateDic stateData = new AnimationStateDic();
        Controller = controller;

        foreach (var layer in controller.layers)
        {
            if (layer.name != targetLayerName) continue;
            for (int i = 0; i < layer.stateMachine.stateMachines.Count(); i++)
            {
                var subMachine = layer.stateMachine.stateMachines[i];
                var stateName = subMachine.stateMachine.name;
                if (stateName != targetSubStateName) continue;

                ProcessStateMachine(subMachine.stateMachine, stateData, controller);
                SetTransitionParameter(stateName, subMachine.stateMachine.entryTransitions, out List<string> nextAnimationNames);
                var newNode = new AnimationComboStateNode(true, stateName, nextAnimationNames);
                aniStateNode.Add(stateName, newNode);
            }
        }

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
    }

    // adding all node
    private void ProcessStateMachine(AnimatorStateMachine stateMachine, AnimationStateDic stateData, AnimatorController controller)
    {
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            ProcessStateMachine(subStateMachine.stateMachine, stateData, controller);
            SetTransitionParameter(subStateMachine.stateMachine.name, subStateMachine.stateMachine.entryTransitions, out List<string> nextAnimationNames);
            var newNode = new AnimationComboStateNode(true, subStateMachine.stateMachine.name, nextAnimationNames);
            if (newNode.hasLoop) LoopList.Add(newNode);

            aniStateNode.Add(subStateMachine.stateMachine.name, newNode);
        }

        foreach (var state in stateMachine.states)
        {
            if (stateData != null)
            {
                SetTransitionParameter(state.state.name, state.state.transitions, out List<string> nextAnimationNames);

                var newNode = new AnimationComboStateNode(false, state.state.name, nextAnimationNames);
                if (newNode.hasLoop) LoopList.Add(newNode);

                aniStateNode.Add(state.state.name, newNode);
            }
        }
    }

    public void SetTransitionParameter(string stateName, AnimatorStateTransition[] transitions, out List<string> nextAnimationNames)
    {
        nextAnimationNames = new List<string>();
        foreach (var transition in transitions)
        {
            var addied = false;
            if (transition.destinationState != null)
            {
                nextAnimationNames.Add(transition.destinationState.name);
                addied = true;

            }
            else if (transition.destinationStateMachine != null)
            {
                nextAnimationNames.Add(transition.destinationStateMachine.name);
                addied = true;
            }

            if (addied)
            {
                var stateNameAlreadyExist = false;
                foreach (var parameter in Controller.parameters)
                {
                    if (parameter.name == stateName)
                    {
                        stateNameAlreadyExist = true;
                        break;
                    }
                }

                if (!stateNameAlreadyExist)
                    Controller.AddParameter(stateName, AnimatorControllerParameterType.Int);

                AnimatorCondition condition = new AnimatorCondition
                {
                    mode = AnimatorConditionMode.Equals,
                    parameter = stateName,
                    threshold = nextAnimationNames.Count - 1
                };

                transition.conditions = new AnimatorCondition[] { condition };
                AddiedParametaList.Add(stateName);
            }
        }
    }

    public void SetTransitionParameter(string stateName, AnimatorTransition[] transitions, out List<string> nextAnimationNames)
    {
        nextAnimationNames = new List<string>();
        foreach (var transition in transitions)
        {
            var addied = false;
            if (transition.destinationState != null)
            {
                nextAnimationNames.Add(transition.destinationState.name);
                addied = true;

            }
            else if (transition.destinationStateMachine != null)
            {
                nextAnimationNames.Add(transition.destinationStateMachine.name);
                addied = true;
            }

            if (addied)
            {
                var stateNameAlreadyExist = false;
                foreach (var parameter in Controller.parameters)
                {
                    if (parameter.name == stateName)
                    {
                        stateNameAlreadyExist = true;
                        break;
                    }
                }

                if (!stateNameAlreadyExist)
                    Controller.AddParameter(stateName, AnimatorControllerParameterType.Int);

                AnimatorCondition condition = new AnimatorCondition
                {
                    mode = AnimatorConditionMode.Equals,
                    parameter = stateName,
                    threshold = nextAnimationNames.Count - 1
                };

                transition.conditions = new AnimatorCondition[] { condition };
                AddiedParametaList.Add(stateName);
            }
        }
    }

    public void CheckRequireNodeExist(List<string> requireNodeNames)
    {
        var missingNode = new List<string>();
        foreach (var requireNodeName in requireNodeNames)
        {
            var find = false;
            foreach (var node in aniStateNode.Keys)
            {
                if (requireNodeName == node)
                {
                    find = true;
                    break;
                }
            }

            if (!find) missingNode.Add(requireNodeName);
        }

        if (missingNode.Count > 0)
        {
            Debug.Log("some requireNodes are missing");
            string list = "";
            missingNode.ForEach(x => list += x + ", ");
        }
    }

    public AnimationComboStateNode GetStateNode(string aniNodeName)
    {
        if (aniStateNode.ContainsKey(aniNodeName))
            return aniStateNode[aniNodeName].Copy();
        return null;
    }

    public void RemoveAddiedParameter()
    {
        AddiedParametaList.ForEach(x =>
        {
            var parameters = Controller.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (x == parameters[i].name)
                {
                    Controller.RemoveParameter(i--);
                }
            }
        });
    }
}