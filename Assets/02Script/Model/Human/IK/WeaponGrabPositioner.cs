using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using JExtentioner;
using UnityEngine.Rendering;
using Unity.Mathematics;
public class InteractionObjLimbIKHandPositioner : MonoBehaviour
{
    public GameObject OriginalPrefab;
    public SerializedDictionary<HandPositioner.HoldingState, HandPositioner> handsGrabPositioner;

    public void SetHandPosition()
    {
        List<Transform> holdingGroup = new List<Transform>
        {
            transform.Find("Holding"),
            transform.Find("Using"),
        };

        for (HandPositioner.HoldingState i = 0; i <= HandPositioner.HoldingState.Using; i++)
        {
            var targetGroup = holdingGroup[(int)i];
            if (targetGroup != null)
            {
                var LHand = targetGroup.Find("LHand");
                var RHand = targetGroup.Find("RHand");
                if (LHand && RHand != null)
                {
                    var eachHand = new SerializedDictionary<HumanBodyBones, Transform>
                    {
                        { HumanBodyBones.LeftHand, LHand },
                        { HumanBodyBones.RightHand, RHand }
                    };

                    var handPositioner = new HandPositioner
                    {
                        holdingState = i,
                        eachHand = eachHand,
                    };

                    handsGrabPositioner.Add(i, handPositioner);
                }
            }
        }
    }

    public void GeneratePositioner()
    {
        for (HandPositioner.HoldingState i = 0; i <= HandPositioner.HoldingState.Using; i++)
        {
            var group = new GameObject(i.ToString());
            var lhand = new GameObject("LHand");
            var rhand = new GameObject("RHand");

            lhand.transform.SetParent(group.transform);
            lhand.transform.position = Vector3.zero;
            lhand.transform.rotation = quaternion.identity;
            lhand.transform.localScale = Vector3.one;

            rhand.transform.SetParent(group.transform);
            rhand.transform.position = Vector3.zero;
            rhand.transform.rotation = quaternion.identity;
            rhand.transform.localScale = Vector3.one;

            group.transform.position = Vector3.zero;
            group.transform.rotation = quaternion.identity;
            group.transform.localScale = Vector3.one;
        }

        SetHandPosition();
    }

    public void ClearSetUp()
    {
        handsGrabPositioner.Clear();
    }

    [Serializable]
    public class HandPositioner
    {
        public enum HoldingState { Holding, Using, Non }
        public HoldingState holdingState = HoldingState.Holding;
        public SerializedDictionary<HumanBodyBones, Transform> eachHand = new SerializedDictionary<HumanBodyBones, Transform>();
    }
}



[CustomEditor(typeof(InteractionObjLimbIKHandPositioner))]
public class WeaponGrabPositionerEditor : Editor
{
    InteractionObjLimbIKHandPositioner weaponGrabPositioner;
    private void Awake()
    {
        weaponGrabPositioner = target as InteractionObjLimbIKHandPositioner;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (weaponGrabPositioner == null) return;

        if (weaponGrabPositioner.OriginalPrefab == null)
            EditorGUILayout.HelpBox("originalPrefab shouldnt empty", MessageType.Warning);

        if (GUILayout.Button("Auto Set UP"))
        {
            weaponGrabPositioner.SetHandPosition();
        }
        EditorGUILayout.HelpBox("for the 'Auto Set UP', there should be have tranform that has 2 child. each child name should be RHand, LHand. and parent name should be Using or Holding", MessageType.Info);

        if (GUILayout.Button("Generate SetUp"))
        {
            weaponGrabPositioner.GeneratePositioner();
        }
        EditorGUILayout.HelpBox("will creat and set positioner auto-matically", MessageType.Info);

        if (weaponGrabPositioner.handsGrabPositioner.Count != 0)
        {
            if (GUILayout.Button("Clear SetUp"))
            {
                weaponGrabPositioner.ClearSetUp();
            }
            EditorGUILayout.HelpBox("use this when remove the list", MessageType.Info);
        }
    }
}