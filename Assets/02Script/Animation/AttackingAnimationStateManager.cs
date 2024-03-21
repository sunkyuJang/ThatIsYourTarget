using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Threading;
using Unity.VisualScripting;

public class AttackingAnimationStateManager : MonoBehaviour
{
    public AnimatorController controller;
    [SerializeField] private SerializedDictionary<string, AnimationStateNode> aniStateNode = new SerializedDictionary<string, AnimationStateNode>();
    private List<AnimationStateNode> LoopList = new List<AnimationStateNode>();
    private static string targetLayerName = "WeaponMotion";
    private static string targetSubStateName = "AttackingWeapon";
    private static string tagForAttack = "Attack";
    public AnimationStateNode GetStartNode() => aniStateNode[targetSubStateName];
    [SerializeField] private List<string> AddiedParametaList = new List<string>();
    public SkillManager skillManager;
    public void InitializeStateData(AnimatorController controller)
    {
        skillManager = new GameObject("SkillManager").AddComponent<SkillManager>();
        skillManager.transform.SetParent(transform);
        skillManager.Initialize(controller);

        // set all attck state
        aniStateNode.Clear();
        LoopList.Clear();
        AddiedParametaList.Clear();
        AnimationStateDic stateData = new AnimationStateDic();
        this.controller = controller;

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
                var newNode = new AnimationStateNode(true, stateName, nextAnimationNames);
                aniStateNode.Add(stateName, newNode);
            }
        }

        foreach (var node in aniStateNode.Values)
        {
            foreach (var nextAnimation in node.nextAnimations)
            {
                var targetNode = GetStateNode(nextAnimation);
                if (targetNode.nowAnimation != node.nowAnimation)
                {
                    targetNode.beforeAnimation = node.nowAnimation;
                }
            }
        }

        skillManager.CheckAllSkillName(aniStateNode.Keys.ToList());
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
            var newNode = new AnimationStateNode(true, subStateMachine.stateMachine.name, nextAnimationNames);
            if (newNode.hasLoop) LoopList.Add(newNode);

            aniStateNode.Add(subStateMachine.stateMachine.name, newNode);
        }

        foreach (var state in stateMachine.states)
        {
            if (stateData != null)
            {
                state.state.tag = tagForAttack;
                SetTransitionParameter(state.state.name, state.state.transitions, out List<string> nextAnimationNames);

                var newNode = new AnimationStateNode(false, state.state.name, nextAnimationNames);
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
                foreach (var parameter in controller.parameters)
                {
                    if (parameter.name == stateName)
                    {
                        stateNameAlreadyExist = true;
                        break;
                    }
                }

                if (!stateNameAlreadyExist)
                    controller.AddParameter(stateName, AnimatorControllerParameterType.Int);

                AnimatorCondition trassitionCondition = new AnimatorCondition
                {
                    mode = AnimatorConditionMode.Equals,
                    parameter = stateName,
                    threshold = nextAnimationNames.Count - 1
                };

                AnimatorCondition OnlyStateInAttackCondition = new AnimatorCondition
                {
                    mode = AnimatorConditionMode.If,
                    parameter = "Attack",
                    threshold = 0,
                };

                transition.conditions = new AnimatorCondition[] { trassitionCondition, OnlyStateInAttackCondition };
                if (!AddiedParametaList.Contains(stateName))
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
                foreach (var parameter in controller.parameters)
                {
                    if (parameter.name == stateName)
                    {
                        stateNameAlreadyExist = true;
                        break;
                    }
                }

                if (!stateNameAlreadyExist)
                    controller.AddParameter(stateName, AnimatorControllerParameterType.Int);

                AnimatorCondition trassitionCondition = new AnimatorCondition
                {
                    mode = AnimatorConditionMode.Equals,
                    parameter = stateName,
                    threshold = nextAnimationNames.Count - 1
                };

                AnimatorCondition OnlyStateInAttackCondition = new AnimatorCondition
                {
                    mode = AnimatorConditionMode.If,
                    parameter = "Attack",
                    threshold = 0,
                };

                transition.conditions = new AnimatorCondition[] { trassitionCondition, OnlyStateInAttackCondition };
                if (!AddiedParametaList.Contains(stateName))
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
            Debug.Log(list);

        }
    }

    public AnimationStateNode GetStateCopyNode(string aniNodeName)
    {
        return GetStateNode(aniNodeName).Copy();
    }
    public AnimationStateNode GetStateNode(string aniNodeName)
    {
        if (aniStateNode.ContainsKey(aniNodeName))
            return aniStateNode[aniNodeName];
        return null;
    }
    public void RemoveAddiedParameter()
    {
        AddiedParametaList.ForEach(x =>
        {
            for (int i = 0; i < controller.parameters.Length; i++)
            {
                if (x == controller.parameters[i].name)
                {
                    controller.RemoveParameter(i--);
                }
            }
        });
    }
}