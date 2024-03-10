using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Threading;
using System;
using UnityEngine.Rendering;

public class InteractionObjHolsterHandler : MonoBehaviour
{
    // keeping != On Hands
    // Holding == On Hands with stay idle 
    // Using ==  OnHands with other stat
    // After Remapping, this list will not use.
    [SerializeField] private List<InteractionObjHolster> Keepingholsters = new List<InteractionObjHolster>();
    [SerializeField] private List<InteractionObjHolster> Holdingholsters = new List<InteractionObjHolster>();
    [SerializeField] private SerializedDictionary<HumanBodyBones, HumanHandLimbIKSetter> personHandLimbIKSetter;
    [SerializeField] private LimbIKPositionerHandler limbIKPositionerHandler;
    [SerializeField] private bool shouldMatch = true;
    public InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState holdingState = InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState.Non;

    // Remap
    private Dictionary<GameObject, HolsterHandlerPair> HolsterPairs { set; get; } = new Dictionary<GameObject, HolsterHandlerPair>();
    public HolsterHandlerPair GetHolsterHandlerPair(GameObject gameObject)
    {
        var originalPrefab = gameObject;
        return HolsterPairs.ContainsKey(originalPrefab) ? HolsterPairs[originalPrefab] : null;
    }
    [SerializeField] private List<InteractionObj> interactionObjs = new List<InteractionObj>();

    //change weight
    [SerializeField] private float changingWeightTime = 0f;

    private void Awake()
    {
        if (TrySetAllHolsterList())
        {
            RemappingAllHandler();
        }
        else
        {
            Debug.Log("somthing wrong when remapping");
        }
    }
    public bool TrySetAllHolsterList()
    {
        Keepingholsters.Clear();
        Holdingholsters.Clear();
        HolsterPairs.Clear();
        personHandLimbIKSetter.Clear();

        var isSucced = true;
        var holsterList = transform.GetComponentsInChildren<InteractionObjHolster>().ToList();
        holsterList.ForEach(x =>
        {
            if (x.holsterStateFor == InteractionObjHolster.State.Non)
            {
                Debug.LogError("Holster State Shouldnt Non");
                isSucced = false;
            }
            else
            {
                var target = x.holsterStateFor == InteractionObjHolster.State.Keeping ? Keepingholsters : Holdingholsters;
                x.SetPositioner();
                target.Add(x);
            }
        });

        var iKSetters = transform.GetComponentsInChildren<HumanHandLimbIKSetter>();
        if (iKSetters != null && iKSetters.Length > 1)
        {
            foreach (var setter in iKSetters)
            {
                personHandLimbIKSetter.Add(setter.handBone, setter);
            }
        }
        else
        {
            isSucced = false;
            Debug.LogError("cantFind PersonHandLimbIKSetter");
        }

        limbIKPositionerHandler = transform.GetComponentInChildren<LimbIKPositionerHandler>();
        if (limbIKPositionerHandler == null)
        {
            isSucced = false;
            Debug.LogError("cantFind LimbIKPositionerHandler");
        }
        else
        {
            limbIKPositionerHandler.SetLimbIK();
        }

        return isSucced;
    }

    public void RemappingAllHandler()
    {
        // remapping 
        Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>> keepingHolsters = new Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>>();
        Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>> holdingHolsters = new Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>>();
        var limbIks = limbIKPositionerHandler.eachLimbIKPositioner;

        for (InteractionObjHolster.State state = InteractionObjHolster.State.Keeping; state <= InteractionObjHolster.State.Non; state++)
        {
            var originalHolsters = state == InteractionObjHolster.State.Keeping ? Keepingholsters : Holdingholsters;
            var remappingHolsters = state == InteractionObjHolster.State.Keeping ? keepingHolsters : holdingHolsters;
            if (originalHolsters != null)
            {
                foreach (var holster in originalHolsters)
                {
                    foreach (var obj in holster.holserRemap.Keys)
                    {
                        remappingHolsters[obj] = new KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>(holster.holserRemap[obj], holster);
                    }
                }
            }
        }

        // matching
        foreach (var key in keepingHolsters.Keys)
        {
            if (holdingHolsters.ContainsKey(key))
            {
                if (limbIks.ContainsKey(key))
                {
                    var targetlimbIK = limbIks[key];
                    HolsterPairs[key] = new HolsterHandlerPair(keepingHolsters[key].Value, holdingHolsters[key].Value, targetlimbIK);
                }
            }
        }

        // if (shouldMatch && (keepingHolsters.Count != holdingHolsters.Count))
        // {
        //     Debug.Log("holster isn't match");
        // }

        LoadAllItemFromHandler();
    }

    public void LoadAllItemFromHandler()
    {
        interactionObjs.Clear();
        Keepingholsters.ForEach(x => { var objList = x.GetInteractionObj(); if (objList != null) objList.ForEach(j => interactionObjs.Add(j)); });
        Holdingholsters.ForEach(x => { var objList = x.GetInteractionObj(); if (objList != null) objList.ForEach(j => interactionObjs.Add(j)); });
    }

    public List<T> GetInteractionObj<T>() where T : InteractionObj
    {
        var list = new List<T>();
        interactionObjs.ForEach(x =>
        {
            if (x is T)
            {
                list.Add(x as T);
            }
        });
        return list;
    }

    public bool SetInteractionObj(InteractionObj interactionObj, InteractionObjHolster.State state)
    {
        if (interactionObjs.Contains(interactionObj)) return false;
        var pair = GetHolsterHandlerPair(interactionObj.originalPrefab);
        if (pair == null) return false;
        if (pair.IsUsing) return false;

        var isSucced = pair.GetHandlerByState(state).TryHold(interactionObj);
        return isSucced;
    }

    public bool SetKeep(InteractionObj interactionObj)
    {
        var isSucced = TrySwitchToTargetHandler(interactionObj, InteractionObjHolster.State.Keeping, out HolsterHandlerPair pair);
        if (!isSucced) return false;

        personHandLimbIKSetter[HumanBodyBones.LeftHand].TurnOnIK(false, changingWeightTime, null, null);
        personHandLimbIKSetter[HumanBodyBones.RightHand].TurnOnIK(false, changingWeightTime, null, null);

        holdingState = InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState.Non;
        return true;
    }

    public bool SetHold(InteractionObj interactionObj, InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState grabbingState, FingerPositioner lfingerPositioner, FingerPositioner rfingerPositioner)
    {
        var isSucced = TrySwitchToTargetHandler(interactionObj, InteractionObjHolster.State.Holding, out HolsterHandlerPair pair);
        if (!isSucced) return false;

        var positioner = pair.limbIKPositioner.handsGrabPositioner[grabbingState];
        var lhandPositiner = positioner.eachHand[HumanBodyBones.LeftHand];
        var rhandPositiner = positioner.eachHand[HumanBodyBones.RightHand];

        personHandLimbIKSetter[HumanBodyBones.LeftHand].TurnOnIK(true, changingWeightTime, lhandPositiner, lfingerPositioner);
        personHandLimbIKSetter[HumanBodyBones.RightHand].TurnOnIK(true, changingWeightTime, rhandPositiner, rfingerPositioner);

        holdingState = grabbingState;
        return true;
    }

    public InteractionObjLimbIKHandPositioner.HandPositioner.HoldingState GetHoldingState()
    {
        return holdingState;
    }

    bool TrySwitchToTargetHandler(InteractionObj interactionObj, InteractionObjHolster.State targetState, out HolsterHandlerPair pair)
    {
        var nowState = targetState == InteractionObjHolster.State.Keeping ? InteractionObjHolster.State.Holding : InteractionObjHolster.State.Keeping;
        pair = GetHolsterHandlerPair(interactionObj.originalPrefab);
        if (pair == null) return false;

        var nowHandler = pair.GetHandlerByState(nowState);
        var targetHandler = pair.GetHandlerByState(targetState);
        if (targetHandler.IsUsing)
        {
            if (targetState == InteractionObjHolster.State.Holding)
            {
                return true;
            }
            Debug.Log("target holster is already using.");
            return false;
        }
        if (!nowHandler.IsUsing)
        {
            Debug.Log("now handler is not using.");
            return false;
        }

        var isSucced = nowHandler.TryRemove(interactionObj);
        if (!isSucced) return false;

        return targetHandler.TryHold(interactionObj);
    }

    [Serializable]
    public class HolsterHandlerPair
    {
        public GameObject originalPrefab;
        public InteractionObjHolster KeepingHolsterHandler { private set; get; }
        public InteractionObjHolster HoldingHolsterHandler { private set; get; }
        public InteractionObjLimbIKHandPositioner limbIKPositioner { private set; get; }
        public InteractionObjHolster GetHandlerByState(InteractionObjHolster.State state) { return state == InteractionObjHolster.State.Keeping ? KeepingHolsterHandler : HoldingHolsterHandler; }
        public bool IsUsing { get => KeepingHolsterHandler.IsUsing || HoldingHolsterHandler.IsUsing; }

        public HolsterHandlerPair(
            InteractionObjHolster keepingholsterHandler,
            InteractionObjHolster holdingHolsterHandler,
            InteractionObjLimbIKHandPositioner limbIKPositioner)
        {
            KeepingHolsterHandler = keepingholsterHandler;
            HoldingHolsterHandler = holdingHolsterHandler;
            this.limbIKPositioner = limbIKPositioner;
        }
    }
}

[CustomEditor(typeof(InteractionObjHolsterHandler))]
public class InteractionObjTotalManagerEditor : Editor
{
    // SerializedProperty personHandLimbIKSetterProperty;
    // private void OnEnable()
    // {
    //     personHandLimbIKSetterProperty = serializedObject.FindProperty("personHandLimbIKSetter");
    // }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector
        InteractionObjHolsterHandler script = (InteractionObjHolsterHandler)target;

        if (GUILayout.Button("Set Each Components"))
        {
            if (script.TrySetAllHolsterList())
            {
                script.RemappingAllHandler();
            }
        }
    }
}