using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using JExtentioner;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.Rendering;
public class AnimatorStateManager : MonoBehaviour
{
    private static AnimatorStateManager instance;
    public static AnimatorStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                InitializeInstance();
                //instance.InitializeStateData();
            }
            return instance;
        }
    }
    [SerializeField] private List<AnimatorController> animators;
    [SerializeField] private SerializedDictionary<AnimatorController, StateData> serializerDictionary = new SerializedDictionary<AnimatorController, StateData>();
    public static void InitializeInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<AnimatorStateManager>();
        }
    }
    public void InitializeStateData()
    {
        foreach (var stateData in serializerDictionary.Values)
        {
            stateData.attackingStateManager.RemoveAddiedParameter();
        }
        serializerDictionary.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        foreach (var controller in animators)
        {
            var group = new GameObject(controller.name);
            var attackingComboManager = new GameObject("AttackingCombo").AddComponent<AttackingAnimationStateManager>();
            var animatorStateManager = new GameObject("AnimatorStateList").AddComponent<AnimatorStateListManager>();
            group.transform.SetParent(transform);
            attackingComboManager.transform.SetParent(group.transform);
            animatorStateManager.transform.SetParent(group.transform);

            attackingComboManager.InitializeStateData(controller);
            animatorStateManager.InitializeStateData(controller);

            var data = new StateData()
            {
                controller = controller,
                attackingStateManager = attackingComboManager,
                animatorStateListManager = animatorStateManager,
            };

            serializerDictionary.Add(controller, data);
        }
    }

    public AnimationStateInfo GetStateInfo(string animator, string stateName) => GetStateInfo(animators.Find(x => x.name == animator), stateName);

    public AnimationStateInfo GetStateInfo(AnimatorController animator, string stateName)
    {
        if (serializerDictionary.ContainsKey(animator))
        {
            var listManager = serializerDictionary[animator].animatorStateListManager;
            if (listManager.serializerDictionary.ContainsKey(stateName))
            {
                return listManager.serializerDictionary[stateName];
            }
        }

        return null;
    }

    public AttackingAnimationStateManager GetAttackingState(string animatorName) => GetAttackingStateManager(animators.Find(x => x.name == animatorName));
    public AttackingAnimationStateManager GetAttackingStateManager(AnimatorController animator)
    {
        if (serializerDictionary.ContainsKey(animator))
        {
            return serializerDictionary[animator].attackingStateManager;
        }

        return null;
    }

    [Serializable]
    public class StateData
    {
        [HideInInspector] public AnimatorController controller;
        public AttackingAnimationStateManager attackingStateManager;
        public AnimatorStateListManager animatorStateListManager;
    }
}
[CustomEditor(typeof(AnimatorStateManager))]
public class AnimatorStateManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();  // 기본 인스펙터 UI를 그립니다.

        AnimatorStateManager manager = (AnimatorStateManager)target;

        if (GUILayout.Button("Initialize State Data"))
        {
            manager.InitializeStateData();
        }
    }
}
